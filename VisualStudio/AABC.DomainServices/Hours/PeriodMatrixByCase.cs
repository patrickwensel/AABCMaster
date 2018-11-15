using System;
using System.Collections.Generic;



namespace AABC.DomainServices.Hours
{
    public class PeriodMatrixByCase
    {


        public static PeriodMatrixByCase GetPeriodMatrix(int caseID, DateTime startDate, DateTime endDate) {

            List<Data.Models.Sprocs.PeriodHoursMatrixByCase> entities = new Data.Services.HoursService().PeriodHoursMatrix(caseID, startDate, endDate);

            var model = new PeriodMatrixByCase();

            model.CaseID = caseID;

            foreach (var e in entities) {

                Hours h;

                switch (e.HoursType) {
                    case "AllHours":
                        h = model.AllHours;
                        break;
                    case "BCBAHours":
                        h = model.BCBAHours;
                        break;
                    case "AideHours":
                        h = model.AideHours;
                        break;
                    default:
                        throw new InvalidOperationException("Hours Type is not registered");
                }

                h.Total = e.TotalHours;
                h.Payable = e.PayableHours;
                h.Billable = e.BillableHours;

            }

            return model;

        }




        public int CaseID { get; set; }
        
        public Hours AllHours { get; set; }
        public Hours BCBAHours { get; set; }
        public Hours AideHours { get; set; }

        public PeriodMatrixByCase() {
            AllHours = new Hours();
            BCBAHours = new Hours();
            AideHours = new Hours();
        }


        public class Hours
        {
            public decimal Total { get; set; }
            public decimal Payable { get; set; }
            public decimal Billable { get; set; }
        }

    }
}
