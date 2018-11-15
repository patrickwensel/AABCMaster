using AABC.Domain.Cases;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.ProviderPortal.Models.Home
{
    public class CaseGeneralHoursVM
    {
                
        const int MAX_MONTH_SPREAD = 12;

        public int DisplayMonths { get; private set; }

        public List<CaseGeneralHoursListItemVM> Items { get; set; }

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










    public class CaseGeneralHoursListItemVM : CaseAuthorization
    {

        public WebViewHelper ViewHelper { get; set; }

        public CaseGeneralHoursListItemVM() {
            ViewHelper = new WebViewHelper(this);

        }


        public class WebViewHelper : IWebViewHelper
        {

            CaseGeneralHoursListItemVM parent;

            public WebViewHelper(CaseGeneralHoursListItemVM parent) {
                this.parent = parent;
            }

            public string AuthClassCode {
                get
                {
                    return parent.AuthClass.Code;
                }
            }

            public double WeeklyHoursAverage {
                get
                {


                    try {
                        int days = (parent.EndDate - parent.StartDate).Days;
                        return parent.TotalHoursApproved / days * 7;
                    }
                    catch {
                        return 0;
                    }
                }
            }

            public string RemainingWeeklyHoursAverage {
                get
                {
                    try {

                        int days = 0;

                        if (parent.StartDate > DateTime.Now) {
                            days = (parent.EndDate - parent.StartDate).Days;
                        } else {
                            days = (parent.EndDate - DateTime.Now).Days;
                        }

                        if (days < 1) {
                            return "N/A";
                        }

                        double d = parent.GeneralHoursRemaining / days * 7;

                        if (d < 0) {
                            return "0";
                        } else {
                            return d.ToString("0.##");
                        }

                    }
                    catch {
                        return "0";
                    }
                }
            }


            public double GeneralHoursMonth1 { get { return getGeneralHours(1); } }
            public double GeneralHoursMonth2 { get { return getGeneralHours(2); } }
            public double GeneralHoursMonth3 { get { return getGeneralHours(3); } }
            public double GeneralHoursMonth4 { get { return getGeneralHours(4); } }
            public double GeneralHoursMonth5 { get { return getGeneralHours(5); } }
            public double GeneralHoursMonth6 { get { return getGeneralHours(6); } }
            public double GeneralHoursMonth7 { get { return getGeneralHours(7); } }
            public double GeneralHoursMonth8 { get { return getGeneralHours(8); } }
            public double GeneralHoursMonth9 { get { return getGeneralHours(9); } }
            public double GeneralHoursMonth10 { get { return getGeneralHours(10); } }
            public double GeneralHoursMonth11 { get { return getGeneralHours(11); } }
            public double GeneralHoursMonth12 { get { return getGeneralHours(12); } }
            public double GeneralHoursMonth13 { get { return getGeneralHours(13); } }
            public double GeneralHoursMonth14 { get { return getGeneralHours(14); } }

            public string Month1Caption { get { return getMonthCaption(1); } }
            public string Month2Caption { get { return getMonthCaption(2); } }
            public string Month3Caption { get { return getMonthCaption(3); } }
            public string Month4Caption { get { return getMonthCaption(4); } }
            public string Month5Caption { get { return getMonthCaption(5); } }
            public string Month6Caption { get { return getMonthCaption(6); } }
            public string Month7Caption { get { return getMonthCaption(7); } }
            public string Month8Caption { get { return getMonthCaption(8); } }
            public string Month9Caption { get { return getMonthCaption(9); } }
            public string Month10Caption { get { return getMonthCaption(10); } }
            public string Month11Caption { get { return getMonthCaption(11); } }
            public string Month12Caption { get { return getMonthCaption(12); } }
            public string Month13Caption { get { return getMonthCaption(13); } }
            public string Month14Caption { get { return getMonthCaption(14); } }


            private string getMonthCaption(int month) {
                return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(parent.GeneralHours[month - 1].Month);
            }

            private double getGeneralHours(int month) {
                return parent.GeneralHours[month - 1].Hours;
            }


            public bool HasValidationErrors { get; set; }

            public string ReturnErrorMessage { get; set; }

            public void BindModel() {
                throw new NotImplementedException();
            }

            public bool Validate() {
                throw new NotImplementedException();
            }
        }

    }
}