using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Services
{
    public class CatalystService
    {

        Models.CoreEntityModel context;


        public CatalystService() {
            context = new Models.CoreEntityModel();
        }



        public List<Domain.Catalyst.NoDataByProviderAndCase> GetNoDataByProviderAndCaseItems(DateTime firstDayOfMonthOfPeriod) {

            DateTime startDate = firstDayOfMonthOfPeriod;
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var data = context.Database.SqlQuery<Models.Sprocs.CatalystNoDataByProviderAndCase>(
                "dbo.GetCatalystDataMissingReportByProviderAndCase @StartDate, @EndDate",
                new System.Data.SqlClient.SqlParameter("@StartDate", startDate),
                new System.Data.SqlClient.SqlParameter("@EndDate", endDate)
                ).ToList();

            var items = new List<Domain.Catalyst.NoDataByProviderAndCase>();
            
            foreach (var d in data) {

                var item = new Domain.Catalyst.NoDataByProviderAndCase();

                item.CaseID = d.CaseID;
                item.Dates = d.dates;
                item.PatientFirstName = d.PatientFirstName;
                item.PatientLastName = d.PatientLastName;
                item.ProviderEmail = d.ProviderPrimaryEmail;
                item.ProviderFirstName = d.ProviderFirstName;
                item.ProviderID = d.ProviderID;
                item.ProviderLastName = d.ProviderLastName;
                item.ProviderPhone = d.ProviderPrimaryPhone;

                items.Add(item);
            }

            return items;
            
        }
        

    }
}
