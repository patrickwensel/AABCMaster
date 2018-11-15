using System;
using System.Collections.Generic;

namespace AABC.ProviderPortal.App.Hours.Models
{
    public class CatalystPreloadEntry
    {

        public int ID { get; set; }                 // table PK
        public DateTime ResponseDate { get; set; }  // form response date

        public int CaseID { get; set; }
        public int ProviderID { get; set; }
        
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Domain2.Services.ServiceIDs ServiceID { get; set; }
        public List<Domain2.Services.ServiceIDs> BCBAServices { get; set; }

        public string Notes { get; set; }
        public bool ProviderAgreed { get; set; }
        public bool ParentAgreed { get; set; }



        public static List<CatalystPreloadEntry> GetMockup() {

            var items = new List<CatalystPreloadEntry>();

            items.Add(new CatalystPreloadEntry()
            {
                 ID = 123,
                 ResponseDate = new DateTime(2017, 12, 10),
                 CaseID = 522,
                 ProviderID = 257,
                 Date = new DateTime(2017, 12, 12),
                 StartTime = new DateTime(2017, 12, 12, 4, 0, 0),
                 EndTime = new DateTime(2017, 12, 12, 6, 0, 0),
                 ServiceID = Domain2.Services.ServiceIDs.DirectCare,
                 Notes = "test entry from catalyst 123",
                 ProviderAgreed = true,
                 ParentAgreed = true
            });

            items.Add(new CatalystPreloadEntry()
            {
                ID = 234,
                ResponseDate = new DateTime(2017, 12, 10),
                CaseID = 522,
                ProviderID = 257,
                Date = new DateTime(2017, 12, 12),
                StartTime = new DateTime(2017, 12, 12, 4, 0, 0),
                EndTime = new DateTime(2017, 12, 12, 6, 0, 0),
                ServiceID = Domain2.Services.ServiceIDs.DirectCare,
                Notes = "test entry from catalyst 234",
                ProviderAgreed = true,
                ParentAgreed = true
            });

            items.Add(new CatalystPreloadEntry()
            {
                ID = 345,
                ResponseDate = new DateTime(2017, 12, 10),
                CaseID = 522,
                ProviderID = 257,
                Date = new DateTime(2017, 12, 13),
                StartTime = new DateTime(2017, 12, 13, 4, 0, 0),
                EndTime = new DateTime(2017, 12, 13, 6, 0, 0),
                ServiceID = Domain2.Services.ServiceIDs.DirectCare,
                Notes = "test entry from catalyst 345",
                ProviderAgreed = true,
                ParentAgreed = true
            });

            items.Add(new CatalystPreloadEntry()
            {
                ID = 456,
                ResponseDate = new DateTime(2017, 12, 10),
                CaseID = 522,
                ProviderID = 257,
                Date = new DateTime(2017, 12, 14),
                StartTime = new DateTime(2017, 12, 14, 4, 0, 0),
                EndTime = new DateTime(2017, 12, 14, 6, 0, 0),
                ServiceID = Domain2.Services.ServiceIDs.DirectCare,
                Notes = "test entry from catalyst 456",
                ProviderAgreed = true,
                ParentAgreed = true
            });

            return items;

        }


    }



}
