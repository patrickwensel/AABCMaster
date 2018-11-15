namespace AABC.DomainServices.Staffing
{
    public interface IStaffingService
    {
        bool PerformCheckByProviderId(int providerId);
        bool PerformCheckByCaseId(int caseId);
    }
}