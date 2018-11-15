using System;
using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class TimeBillAddHoursVM
    {

        // prefix cahp to get around dupe name issues and devex binding
        // (cahp: Case Add Hours Popup)
        public AABC.Domain.Providers.Provider cahpProvider { get; set; }
        public List<AABC.Domain.Providers.Provider> cahpProviderList { get; set; }
        public DateTime cahpDate { get; set; }
        public DateTime cahpTimeIn { get; set; }
        public DateTime cahpTimeOut { get; set; }
        public Domain.Cases.Service cahpService { get; set; }
        public List<Domain.Cases.Service> cahpServiceList { get; set; }
        public string cahpNotes { get; set; }
        public bool cahpIsPayOrBillAdjustment { get; set; }

        
    }
}