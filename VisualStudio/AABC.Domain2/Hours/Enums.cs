namespace AABC.Domain2.Hours
{
    public enum HoursStatus
    {
        PreChecked = -1,
        Pending = 0,
        ComittedByProvider = 1,
        FinalizedByProvider = 2,
        ScrubbedByAdmin = 3,
        ProcessedComplete = 4
    }
}
