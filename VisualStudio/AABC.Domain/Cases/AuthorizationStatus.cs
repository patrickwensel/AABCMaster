namespace AABC.Domain.Cases
{
    // maps to dbo.CaseAuthHoursStatuses
    public enum AuthorizationHoursStatus
    {
        PreCheckedOnApp = -1,
        Pending = 0,
        ComittedByProvider = 1,
        FinalizedByProvider = 2,
        ScrubbedByAdmin = 3,
        ProcessedComplete = 4
    }

    public class AuthorizationStatus
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
