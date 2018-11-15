using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.Models.Cases
{
    public class GeneralHoursVM
    {

        const int MAX_MONTH_SPREAD = 12;

        public int DisplayMonths { get; private set; }

        public List<GeneralHoursListItemVM> Items { get; set; }


        public int CaseID { get; set; }

        public void FillListBlanks() {

            // default to today and +6mo
            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddMonths(6);

            if (Items.Count > 0) {
                startDate = getLatestStartDate();
                endDate = getLatestEndDate();
            }
            
            // accept 14 month max diff
            if ((((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month) > MAX_MONTH_SPREAD) {
                throw new ArgumentOutOfRangeException("Exceeds max span of " + MAX_MONTH_SPREAD + " months between start and end date");
            }

            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            endDate = new DateTime(endDate.Year, endDate.Month, 1);

            startDate = startDate.AddMonths(-1);
            endDate = endDate.AddMonths(1);

            DisplayMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1;

            var addMonths = MAX_MONTH_SPREAD + 2 - DisplayMonths;
            endDate = endDate.AddMonths(addMonths);

            fillListBlanks(startDate, endDate);

        }
        

        private void fillListBlanks(DateTime startDate, DateTime endDate) {
            // start and end date should be first day of the month

            foreach (var item in Items) {

                DateTime d = startDate;
                DateTime endLoop = endDate.AddDays(1);
                while (d < endLoop) {

                    // if not present, create a blank one

                    var hours = item.GeneralHours.Where(x => x.Year == d.Year && x.Month == d.Month).FirstOrDefault();
                    if (hours == null) {
                        hours = new Domain.Cases.CaseAuthorizationGeneralHours();
                        hours.Year = d.Year;
                        hours.Month = d.Month;
                        hours.Hours = 0;
                        item.GeneralHours.Add(hours);
                    }

                    d = d.AddMonths(1);
                }

                // clean up any existing hours that aren't part of our range
                item.GeneralHours.RemoveAll(x => x.Year < startDate.Year);
                item.GeneralHours.RemoveAll(x => x.Year > endDate.Year);
                item.GeneralHours.RemoveAll(x => x.Month < startDate.Month && x.Year <= startDate.Year);
                item.GeneralHours.RemoveAll(x => x.Month > endDate.Month && x.Year >= endDate.Year);

                item.GeneralHours = item.GeneralHours.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
            }

        }

        private DateTime getLatestStartDate() {
            return Items.OrderByDescending(x => x.StartDate).First().StartDate;
        }

        private DateTime getLatestEndDate() {
            return Items.OrderByDescending(x => x.EndDate).First().EndDate;
        }

    }
}