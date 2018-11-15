using System;
using System.Collections.Generic;

namespace AABC.Web.Services
{

    public class HoursAutopopulateService
    {

        private Repositories.AutoPopulateHoursRepository repository;
        private readonly string connectionString;

        public HoursAutopopulateService() {
            repository = new Repositories.AutoPopulateHoursRepository();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }


        public void PopulateHours(List<Domain.Cases.CaseAuthorizationHours> items) {
            repository.AddAutopopulatedHours(items);
        }

        public int RemoveInvalidEntriesFromHoursSet(List<Domain.Cases.CaseAuthorizationHours> items) {

            if (items.Count == 0) {
                return 0;
            }

            Domain.Cases.Case c = repository.GetCase(items[0].CaseID.Value);
            c.Authorizations = repository.GetCaseAuthorizations(c.ID.Value);


            var removals = new List<Domain.Cases.CaseAuthorizationHours>();

            foreach (var item in items) {

                bool isValidEntry = true;

                var resolver = new DomainServices.Providers.HoursEntryResolver(c, item);

                resolver.Resolve();

                if (resolver.PassingStatus < DomainServices.Providers.HoursEntryResolver.ValidationResultStatus.ProviderFinalized) {
                    isValidEntry = false;
                }

                if (isValidEntry) {
                    // TODO: add to auth for next item processing?

                } else {
                    removals.Add(item);
                }

            }

            items.RemoveAll(x => removals.Contains(x));

            return removals.Count;


        }


        public List<Domain.Cases.CaseAuthorizationHours> GenerateHoursSet(DateTime startDate, DateTime endDate, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun) {

            var list = new List<Domain.Cases.CaseAuthorizationHours>();

            DateTime currentDate = startDate;

            while (currentDate <= endDate) {

                DayOfWeek dow = currentDate.DayOfWeek;

                if (dow == DayOfWeek.Monday && mon) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                if (dow == DayOfWeek.Tuesday && tue) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                if (dow == DayOfWeek.Wednesday && wed) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                if (dow == DayOfWeek.Thursday && thu) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                if (dow == DayOfWeek.Friday && fri) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                if (dow == DayOfWeek.Saturday && sat) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                if (dow == DayOfWeek.Sunday && sun) {
                    list.Add(new Domain.Cases.CaseAuthorizationHours() { Date = currentDate });
                }

                currentDate = currentDate.AddDays(1);
            }

            return list;
        }



    }
}
