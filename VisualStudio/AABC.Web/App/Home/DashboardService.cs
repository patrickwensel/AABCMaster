using AABC.Web.App.Hours.Models;
using AABC.Web.App.Models;
using AABC.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Home
{
    public class DashboardService
    {



        internal List<InsuranceCostListItem> Insurance2CostsListItems(DateTime periodStart, int insuranceID)
        {

            DateTime startDate = periodStart;
            DateTime endDate = periodStart.AddMonths(1).AddDays(-1);

            var q = from c in _context.HoursProviderCosts
                    where c.PatientInsuranceID == insuranceID
                        && c.HoursDate >= startDate
                        && c.HoursDate <= endDate
                    group c by new { c.ID, c.PatientFirstName, c.PatientLastName } into g
                    select new InsuranceCostListItem
                    {
                        PatientID = g.Key.ID,
                        PatientName = g.Key.PatientFirstName + " " + g.Key.PatientLastName,
                        Total = g.Sum(x => x.ProviderCost)
                    };

            return q.ToList();

        }


        public List<InsuranceCostListItem> InsuranceCostsListItems(DateTime periodStart, string insuranceName)
        {

            DateTime startDate = periodStart;
            DateTime endDate = periodStart.AddMonths(1).AddDays(-1);

            var q = from c in _context.HoursProviderCosts
                    where c.PatientInsuranceCompanyName == insuranceName
                        && c.HoursDate >= startDate
                        && c.HoursDate <= endDate
                    group c by new { c.ID, c.PatientFirstName, c.PatientLastName } into g
                    select new InsuranceCostListItem
                    {
                        PatientID = g.Key.ID,
                        PatientName = g.Key.PatientFirstName + " " + g.Key.PatientLastName,
                        Total = g.Sum(x => x.ProviderCost)
                    };

            return q.ToList();

        }



        public List<AvailableDate> GetAvailableDates()
        {
            return hoursRepo.GetScrubAvailableDates();
        }

        public AvailableDate DefaultSelectedDate()
        {
            return new AvailableDate()
            {
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            };
        }








        private Repos.HoursRepo hoursRepo;
        private Data.Models.CoreEntityModel _context;

        public DashboardService()
        {
            hoursRepo = new Repos.HoursRepo();
            _context = new Data.Models.CoreEntityModel();
        }

        internal List<CommonLists.InsuranceListItem> GetInsuranceList()
        {
            return CommonLists.Default.Insurances();
        }

        internal List<CommonLists.Insurance2ListItem> GetInsurance2List()
        {
            return CommonLists.Default.Insurances2();
        }


    }
}