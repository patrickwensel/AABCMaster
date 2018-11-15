namespace AABC.Domain
{
    public static class DomainHelpers
    {
        public static string GetCommonName(string firstName, string lastName)
        {
            return $"{firstName} {lastName}".Trim();
        }
    }
}
