using AABC.Domain.Cases;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.Models.Cases
{
    public class GeneralHoursEditVM
    {


        const int MAX_MONTH_SPREAD = 12;

        // put this in a detail object so the naming for the auth edit
        // popup in the devex view doesn't collide with the underlying
        // controls in the parent view
        public CaseAuthorization Detail { get; set; }
                
        public WebViewHelper ViewHelper { get; set; }


        public GeneralHoursEditVM() {
            ViewHelper = new WebViewHelper(this);
        }


        public Authorization NewAuth { get; set; }


        public class WebViewHelper : IWebViewHelper
        {
         
            
            public int DisplayMonths { get; private set; }
               
            public List<AuthorizationClass> AuthClasses { get; set; }
            public List<Authorization> Authorizations { get; set; }
            public int? CaseID { get; set; }

            public double Month1Hours { get; set; }
            public double Month2Hours { get; set; }
            public double Month3Hours { get; set; }
            public double Month4Hours { get; set; }
            public double Month5Hours { get; set; }
            public double Month6Hours { get; set; }
            public double Month7Hours { get; set; }
            public double Month8Hours { get; set; }
            public double Month9Hours { get; set; }
            public double Month10Hours { get; set; }
            public double Month11Hours { get; set; }
            public double Month12Hours { get; set; }
            public double Month13Hours { get; set; }
            public double Month14Hours { get; set; }

            public string Month1Caption { get; set; }
            public string Month2Caption { get; set; }
            public string Month3Caption { get; set; }
            public string Month4Caption { get; set; }
            public string Month5Caption { get; set; }
            public string Month6Caption { get; set; }
            public string Month7Caption { get; set; }
            public string Month8Caption { get; set; }
            public string Month9Caption { get; set; }
            public string Month10Caption { get; set; }
            public string Month11Caption { get; set; }
            public string Month12Caption { get; set; }
            public string Month13Caption { get; set; }
            public string Month14Caption { get; set; }

            private string getMonthCaption(int month) {
                return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(parent.Detail.GeneralHours[month - 1].Month);
            }

            private double getGeneralHours(int month) {
                return parent.Detail.GeneralHours[month - 1].Hours;
            }

            public void InitGeneralHours() {

                Month1Hours = getGeneralHours(1);
                Month2Hours = getGeneralHours(2);
                Month3Hours = getGeneralHours(3);
                Month4Hours = getGeneralHours(4);
                Month5Hours = getGeneralHours(5);
                Month6Hours = getGeneralHours(6);
                Month7Hours = getGeneralHours(7);
                Month8Hours = getGeneralHours(8);
                Month9Hours = getGeneralHours(9);
                Month10Hours = getGeneralHours(10);
                Month11Hours = getGeneralHours(11);
                Month12Hours = getGeneralHours(12);
                Month13Hours = getGeneralHours(13);
                Month14Hours = getGeneralHours(14);

                Month1Caption = getMonthCaption(1);
                Month2Caption = getMonthCaption(2);
                Month3Caption = getMonthCaption(3);
                Month4Caption = getMonthCaption(4);
                Month5Caption = getMonthCaption(5);
                Month6Caption = getMonthCaption(6);
                Month7Caption = getMonthCaption(7);
                Month8Caption = getMonthCaption(8);
                Month9Caption = getMonthCaption(9);
                Month10Caption = getMonthCaption(10);
                Month11Caption = getMonthCaption(11);
                Month12Caption = getMonthCaption(12);
                Month13Caption = getMonthCaption(13);
                Month14Caption = getMonthCaption(14);

            }



            public void FillListBlanks() {

                DateTime startDate = parent.Detail.StartDate;
                DateTime endDate = parent.Detail.EndDate;
                                
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
                // start and end date should be coming in as first day of the month
                
                DateTime d = startDate;
                DateTime endLoop = endDate.AddDays(1);
                while (d < endLoop) {

                    // if not present, create a blank one

                    var hours = parent.Detail.GeneralHours.Where(x => x.Year == d.Year && x.Month == d.Month).FirstOrDefault();
                    if (hours == null) {
                        hours = new CaseAuthorizationGeneralHours();
                        hours.Year = d.Year;
                        hours.Month = d.Month;
                        hours.Hours = 0;
                        parent.Detail.GeneralHours.Add(hours);
                    }

                    d = d.AddMonths(1);
                    
                }

                // clean up any existing hours that aren't part of our range
                parent.Detail.GeneralHours.RemoveAll(x => x.Year < startDate.Year);
                parent.Detail.GeneralHours.RemoveAll(x => x.Year > endDate.Year);
                parent.Detail.GeneralHours.RemoveAll(x => x.Month < startDate.Month && x.Year <= startDate.Year);
                parent.Detail.GeneralHours.RemoveAll(x => x.Month > endDate.Month && x.Year >= endDate.Year);

                parent.Detail.GeneralHours = parent.Detail.GeneralHours.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();

            }



            // boilerplate stuff

            GeneralHoursEditVM parent;

            public WebViewHelper(GeneralHoursEditVM parent) {
                this.parent = parent;
            }
            
            public bool HasValidationErrors { get; set; }

            public string ReturnErrorMessage { get; set; }

            public void BindModel() {

                

                // map the static hours back into general hours list
                parent.Detail.GeneralHours = new List<CaseAuthorizationGeneralHours>();

                FillListBlanks();

                parent.Detail.GeneralHours[0].Hours = Month1Hours;
                parent.Detail.GeneralHours[1].Hours = Month2Hours;
                parent.Detail.GeneralHours[2].Hours = Month3Hours;
                parent.Detail.GeneralHours[3].Hours = Month4Hours;
                parent.Detail.GeneralHours[4].Hours = Month5Hours;
                parent.Detail.GeneralHours[5].Hours = Month6Hours;
                parent.Detail.GeneralHours[6].Hours = Month7Hours;
                parent.Detail.GeneralHours[7].Hours = Month8Hours;
                parent.Detail.GeneralHours[8].Hours = Month9Hours;
                parent.Detail.GeneralHours[9].Hours = Month10Hours;
                parent.Detail.GeneralHours[10].Hours = Month11Hours;
                parent.Detail.GeneralHours[11].Hours = Month12Hours;
                parent.Detail.GeneralHours[12].Hours = Month13Hours;
                parent.Detail.GeneralHours[13].Hours = Month14Hours;

            }

            public bool Validate() {
                if (parent.Detail.StartDate == null || parent.Detail.EndDate == null) {
                    return false;
                }
                return true;    
            }
        }

    }
}