namespace AABC.Data.V2.Procedures
{
    public class CatalystProcedures
    {

        CoreContext _context;
            

        public CatalystProcedures(CoreContext context) {
            _context = context;
        }

        public void MapTimesheetProviders() {
            _context.Database.ExecuteSqlCommand("EXEC cata.MapTimesheetProviders");
        }

        public void MapTimesheetCases() {
            _context.Database.ExecuteSqlCommand("EXEC cata.MapTimesheetCases");
        }

    }
}
