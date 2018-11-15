using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace AABC.DomainServices.Integrations.Catalyst
{
    public class HasData
    {

        // uses EPPlus and SQL Server to do the heavy processing
        // superceeds DataTrackMatrix* methods

        private string _filePath;
        private Data.V2.CoreContext _context;
        

        public HasData(string filePath) {
            _filePath = filePath;
            _context = new Data.V2.CoreContext();
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
        }

        public bool Validate(ref string errorText) {
            var table = getTableFromExcel();
            return validateTable(table, ref errorText);
        }

        public void Import() {

            var table = getTableFromExcel();

            // do this differently if we ever get real volume...
            _context.CatalystHasDataEntries.RemoveRange(_context.CatalystHasDataEntries);
            _context.SaveChanges();

            table.Rows.Remove(table.Rows[0]);   // blank row

            var dateRow = table.Rows[0];
            int colCount = table.Columns.Count;

            int count = 0;

            for (int i = 1; i < table.Rows.Count - 1; i++) {

                string studentName = table.Rows[i][0].ToString();

                for (int j = 1; j < colCount - 1; j++) {

                    DateTime d = getDateFromRawValue(dateRow[j].ToString());
                    var initials = table.Rows[i][j].ToString();
                    if (!string.IsNullOrWhiteSpace(initials.Trim())) {

                        count++;

                        var item = new Domain2.Integrations.Catalyst.HasDataEntry();
                        item.Name = studentName;
                        item.ProviderInitialsSet = initials.Trim();
                        item.Date = d;

                        _context = addToContext(_context, item, count, 100, true);
                    }
                }

            }
            _context.SaveChanges();
        }

        public IEnumerable<HasDataResultItem> Resolve() {

            var results = _context.Database.SqlQuery<HasDataResultItem>("EXEC cata.GenerateHasDataResults;");

            return results.ToList();

        }









        private Data.V2.CoreContext addToContext(Data.V2.CoreContext context, Domain2.Integrations.Catalyst.HasDataEntry entity, int count, int commitCount, bool recreateContext) {
            
            //https://stackoverflow.com/questions/5940225/fastest-way-of-inserting-in-entity-framework

            context.Set<Domain2.Integrations.Catalyst.HasDataEntry>().Add(entity);

            if (count % commitCount == 0) {
                context.SaveChanges();
                if (recreateContext) {
                    context.Dispose();
                    context = new Data.V2.CoreContext();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }



        

        private DateTime getDateFromRawValue(string value) {

            int monthPart = int.Parse(value.Split('/')[0]);
            int dayPart = int.Parse(value.Split('/')[1]);
            int currentMonth = DateTime.UtcNow.Month;
            int yearPart = DateTime.UtcNow.Year;

            // date is collected from report without a year, so current year is added on parsing.
            // if we're in the first quarter now, and the report is from last quarter,
            // remove a year from the parsed date
            if (currentMonth < 4 && monthPart >= 9) {
                yearPart--;
            }

            return new DateTime(yearPart, monthPart, dayPart, 0, 0, 0);

        }
        
        private DataTable getTableFromExcel() {

            using (var package = new ExcelPackage()) {
                using (var stream = File.OpenRead(_filePath)) {
                    package.Load(stream);
                }
                var sheet = package.Workbook.Worksheets.First();
                var dt = new DataTable();
                for (int i = 0; i < sheet.Dimension.End.Column; i++) {
                    dt.Columns.Add();
                }
                for (int i = 2; i <= sheet.Dimension.End.Row; i++) {
                    var wsRow = sheet.Cells[i, 1, i, sheet.Dimension.End.Column];
                    var dtRow = dt.NewRow();
                    foreach (var cell in wsRow) {
                        dtRow[cell.Start.Column - 1] = cell.Text;
                    }
                    dt.Rows.Add(dtRow);
                }
                return dt;
            }

        }

        bool validateTable(DataTable t, ref string errorText) {
            if (t == null) {
                errorText = "Unable to load to table";
                return false;
            }
            if (t.Rows.Count < 5) { // one for heading (including dates), at least one data row
                errorText = "Less than 5 rows found in the sheet";
                return false;
            }
            if (t.Columns.Count < 3) {  // one for student name, at least one for date
                errorText = "Less than 3 columns found in sheet";
                return false;
            }
            var r = t.Rows[1];
            if (r[0].ToString() != "Student Name") {
                errorText = "Expected caption 'Student Name' not found";
                return false;
            }
            DateTime dateTest;
            for (int i = 1; i < t.Columns.Count; i++) { // ensure second and subsequent columns are dates
                if (!DateTime.TryParse(r[i].ToString(), out dateTest)) {
                    errorText = "Unable to determine date format (colindex: " + i.ToString() + ")";
                    return false;
                }
            }
            return true;
        }


    }
}
