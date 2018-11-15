namespace AABC.DomainServices.HoursResolution
{
    class ScheduleResolution
    {

        private IResolutionService _resolutionService;

        public ScheduleResolution(IResolutionService resolutionService) {
            _resolutionService = resolutionService;
        }

        public bool Resolve() {
            // scheduling not implemented yet...
            return true;
        }
    }
}
