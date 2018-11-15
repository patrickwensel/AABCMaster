using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace AABC.DomainServices.TableExporter
{
    public class TableExporter
    {

        public string ConnectionString { get; set; }

        public TableExporter(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public byte[] GetFile(ExportDefinition exportDefinitions)
        {
            return GetFile(new List<ExportDefinition> { exportDefinitions });
        }

        public byte[] GetFile(IEnumerable<ExportDefinition> exportDefinitions)
        {
            if (exportDefinitions == null) throw new ArgumentNullException(nameof(exportDefinitions));
            byte[] result = null;

            var operations = new List<Tuple<ExportDefinition, DataTable>>();
            foreach (var exp in exportDefinitions)
            {
                if (exp != null)
                {
                    var table = new DataTable();
                    using (var conn = new SqlConnection(ConnectionString))
                    using (var cmd = new SqlCommand(exp.SelectStatement, conn))
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(table);
                    }
                    operations.Add(new Tuple<ExportDefinition, DataTable>(exp, table));
                }
            }

            using (var ms = new MemoryStream())
            using (var pck = new ExcelPackage(ms))
            {
                foreach (var t in operations)
                {
                    if (t.Item2.Rows.Count > 0)
                    {
                        var ws = pck.Workbook.Worksheets.Add(!string.IsNullOrEmpty(t.Item1.WorksheetName) ? t.Item1.WorksheetName : "Sheet");
                        var range = ws.Cells["A1"].LoadFromDataTable(t.Item2, true);
                        if (t.Item1.FormatAsTable)
                        {
                            var excelTable = ws.Tables.Add(range, !string.IsNullOrEmpty(t.Item1.TableRangeName) ? t.Item1.TableRangeName : "Table");
                            excelTable.TableStyle = t.Item1.TableStyle;
                            t.Item1.Format?.Invoke(ws, t.Item2);
                        }
                    }
                }
                result = pck.GetAsByteArray();
            }
            return result;
        }
    }

    public class ExportDefinition
    {
        public string SelectStatement { get; set; }
        public string WorksheetName { get; set; } = "Sheet 1";
        public string TableRangeName { get; set; } = "Table1";
        public bool FormatAsTable { get; set; } = false;
        public TableStyles TableStyle { get; set; } = TableStyles.Light1;   // ignored if not FormatTable
        public Action<ExcelWorksheet, DataTable> Format { get; set; }
    }
}
