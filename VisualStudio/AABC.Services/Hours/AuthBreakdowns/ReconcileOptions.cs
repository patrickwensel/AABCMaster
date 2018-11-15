using System;

namespace AABC.Services.Hours.AuthBreakdowns
{
    public class ReconcileOptions
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool BreakOnExceptionBypassed { get; set; }

    }
}
