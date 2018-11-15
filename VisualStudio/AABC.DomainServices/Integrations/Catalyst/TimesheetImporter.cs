using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace AABC.DomainServices.Integrations.Catalyst
{
    public class TimesheetImporter {

        private string _filePath;
        private Data.V2.CoreContext _context;


        public TimesheetImporter(string filePath) {
            _filePath = filePath;
            _context = new Data.V2.CoreContext();
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
        }


        public void Import() {
            
            var table = getTableFromExcel();

            var entries = new List<Domain2.Integrations.Catalyst.TimesheetPreloadEntry>();

            // ignore if date prior to last entry of existing

            /*
             * COLUMNS
             *  0   A:  Form Name
             *  1   B:  Response Date
             *  2   C:  Student Code
             *  3   D:  Student Name
             *  4   E:  Therapist
             *  5   F:  Date
             *  6   G:  Aide Services
             *  7   H:  BCBA Services
             *  8   I:  Notes
             *  9   J:  ProviderAgrees
             *  10  K: ProviderSigned
             *  11  L: Parent Agrees
             *  12  M: ParentSigned
             *  (all subsequent columns ignored)
             */

            foreach (DataRow row in table.Rows) {
                
                var entry = new Domain2.Integrations.Catalyst.TimesheetPreloadEntry();

                DateTime? resolvedDate = getDate(row[5].ToString());

                if (resolvedDate.HasValue) {
                    entry.Date = resolvedDate.Value;
                    entry.IsResolved = false;
                    entry.MappedCaseID = null;
                    entry.MappedProviderID = null;
                    entry.Notes = row[8].ToString();
                    entry.ParentAgreed = row[11] == null ? false : true;
                    entry.PatientName = row[3].ToString();
                    entry.ProviderAgreed = row[9] == null ? false : true;
                    entry.ProviderName = row[4].ToString();
                    entry.ResponseDate = getResponseDate(double.Parse(row[1].ToString()));

                    entries.Add(entry);
                }
            }

            DateTime lastImportMaxDate = new DateTime(2000, 1, 1, 0, 0, 0);
            if (_context.CatalystPreloadEntries.Count() > 0) {
                lastImportMaxDate = _context.CatalystPreloadEntries.Max(x => x.ResponseDate);
            }

            var validEntries = entries
                .Where(x => !string.IsNullOrWhiteSpace(x.Notes))            
                .Where(x => x.ResponseDate > lastImportMaxDate)
                .ToList();

            // add entries and save them....
            _context.CatalystPreloadEntries.AddRange(validEntries);
            _context.SaveChanges();

            // then map providers and patients
            MapProviders();
            MapCases();
            
        }

        public void MapProviders() {
            _context.CatalystProcedures.MapTimesheetProviders();
        }

        public void MapCases() {
            _context.CatalystProcedures.MapTimesheetCases();
        }


        private DateTime? getDate(string value) {

            try {

                string[] parts = value.Split('/');

                // the last element may have a time attached to it, but we only want the year
                string lastPart = parts[2];
                parts[2] = parts[2].Substring(0, 4);

                int dayPart = int.Parse(parts[1]);
                int monthPath = int.Parse(parts[0]);
                int yearPart = int.Parse(parts[2]);

                return new DateTime(yearPart, monthPath, dayPart, 0, 0, 0);

            }
            catch {
                return null;
            }         
        } 

        private DateTime getResponseDate(double value) {
            return DateTime.FromOADate(value);
        }


        private DataTable getTableFromExcel() {

            using (var package = new ExcelPackage()) {
                using (var stream = File.OpenRead(_filePath)) {
                    package.Load(stream);
                }

                var sheet = package.Workbook.Worksheets.First();
                var table = new DataTable();

                for (int i = 0; i < sheet.Dimension.End.Column; i++) {
                    table.Columns.Add();
                }

                for (int i = 2; i < sheet.Dimension.End.Row; i++) {
                    var wsRow = sheet.Cells[i, 1, i, sheet.Dimension.End.Column];
                    var dtRow = table.NewRow();
                    foreach (var cell in wsRow) {
                        dtRow[cell.Start.Column - 1] = cell.Value.ToString();
                    }
                    table.Rows.Add(dtRow);
                }

                return table;
            }

        }


    }
}
