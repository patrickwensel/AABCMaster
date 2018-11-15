using AABC.Domain.Cases;
using Dymeng.Framework.Web.Mvc.Views;
using System;

namespace AABC.Web.Models.Cases
{

    public class GeneralHoursListItemVM : CaseAuthorization
    {
        
        public WebViewHelper ViewHelper { get; set; }

        public GeneralHoursListItemVM() {
            ViewHelper = new WebViewHelper(this);
            
        }


        public class WebViewHelper : IWebViewHelper
        {

            GeneralHoursListItemVM parent;

            public WebViewHelper(GeneralHoursListItemVM parent) {
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