using AABC.Domain.Cases;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AABC.DomainServices.Hours
{
    public class CrossHoursValidation
    {


        /***********************
         * 
         * FIELDS & PROPERTIES
         * 
         **********************/

        public enum ValidationErrors
        {
            ProviderOverlapSelf,
            CaseOverlap,
            SupervisionMismatch,
            AideMaxHoursPerDayPerAide,
            NoCatalystData,
            NotesMissing
            //AideMaxHoursPerDayPerCase
        }

        Data.Services.ICaseService _caseService;

        Data.Models.CoreEntityModel context;

        public List<CaseAuthorizationHours> Hours { get; set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public List<Error> Errors { get; set; }

        public List<string> ValidationMessages
        {
            get
            {
                var messages = new List<string>();

                Errors.ForEach(x =>
                {
                    string s;
                    s = x.ErrorType.ToString() + " [CID " + x.MatchedOn.CaseID + "][PID " + x.MatchedOn.ProviderID + "]";
                    s += "[HID " + x.MatchedOn.ID + "][SVC-" + x.MatchedOn.Service.Code + "] (" + x.MatchedOn.TimeIn + " - " + x.MatchedOn.TimeOut + ") \t\t";
                    foreach (var m in x.MatchedTo)
                    {
                        s += "[CID " + m.CaseID + "][PID " + m.ProviderID + "][HID " + m.ID + "][SVC-" + m.Service.Code + "] (" + m.TimeIn + " - " + m.TimeOut + ")\t, ";
                    }
                    messages.Add(s);
                });

                return messages;
            }
        }





        /***********************
         * 
         * CTOR/DTOR
         * 
         **********************/


        public CrossHoursValidation(Data.Services.ICaseService caseService, DateTime firstDayOfPeriod)
        {

            Errors = new List<DomainServices.Hours.CrossHoursValidation.Error>();

            StartDate = firstDayOfPeriod;
            EndDate = firstDayOfPeriod.AddMonths(1).AddSeconds(-1);

            _caseService = caseService;

            Hours = _caseService.GetCaseHoursByDateRange(StartDate, EndDate);

            context = new Data.Models.CoreEntityModel();

        }


        /***********************
         * 
         * PUBLIC METHODS
         * 
         **********************/

        public void Validate()
        {

            EnsureNoOverlapWithinCase();
            EnsureNoOverlapWithinProvider();
            EnsureSocialGroupOverlaps();
            EnsureSupervisionOverlaps();
            //ensureMaxXHoursPerDayPerAide();

        }
        public void ValidateMaxHoursPerDayPerAide()
        {
            EnsureMaxXHoursPerDayPerAide();
        }


        /***********************
         * 
         * PRIVATE METHODS
         * 
         **********************/








        void AddError(ValidationErrors error, CaseAuthorizationHours sourceHours, CaseAuthorizationHours correlatedHours)
        {

            // make sure this hasn't been previously matched
            var existing = Errors.Where(x =>
                (x.MatchedOn.ID == correlatedHours?.ID) &&
                (x.ErrorType == error))
                .ToList();

            if (existing.Count == 0)
            {

                Error err = new Error
                {
                    ErrorType = error,
                    MatchedOn = sourceHours
                };
                if (correlatedHours != null)
                {
                    err.MatchedTo.Add(correlatedHours);
                }

                Errors.Add(err);

            }

        }



        // an aide who has more than X (5) hours entered for a single day
        void EnsureMaxXHoursPerDayPerAide()
        {

            var q = (from h in context.CaseAuthHours
                     join p in context.Providers on h.CaseProviderID equals p.ID
                     join pt in context.ProviderTypes on p.ProviderType equals pt.ID
                     where h.HoursDate >= this.StartDate
                         && h.HoursDate <= this.EndDate
                         && pt.ProviderTypeCode == "AIDE"
                     group h by new { h.CaseProviderID, h.HoursDate } into g
                     where g.Sum(x => x.HoursTotal) > 5
                     select new
                     {
                         g.Key.CaseProviderID,
                         g.Key.HoursDate,
                         Hours = g.Sum(x => x.HoursTotal)
                     }).ToList();

            foreach (var o in q)
            {
                var hours = Hours.Where(x => x.ProviderID == o.CaseProviderID && x.Date == o.HoursDate).ToList();
                foreach (var h in hours)
                {
                    AddError(ValidationErrors.AideMaxHoursPerDayPerAide, h, null);
                }
            }

        }


        // a provider who's entered hours overlaps themselves in any case
        void EnsureNoOverlapWithinProvider()
        {

            // group by provider
            var groups = Hours.GroupBy(x => x.ProviderID);

            foreach (var group in groups)
            {

                foreach (var item in group)
                {

                    var checkList = group.Where(x => x.ID != item.ID && x.Date == item.Date).ToList();

                    var matches = checkList.Where(x =>
                        (x.TimeIn < item.TimeOut) &&
                        (x.TimeOut > item.TimeIn)
                        ).ToList();

                    matches.AddRange(checkList.Where(x =>
                        (x.TimeOut < item.TimeOut) &&
                        (x.TimeOut > item.TimeIn)
                        ).ToList());

                    matches = matches.Distinct().ToList();

                    foreach (var match in matches)
                    {
                        // create new validation error
                        AddError(ValidationErrors.ProviderOverlapSelf, item, match);
                    }

                }

            }


        }

        // a case who has hours entered which overlap each other by any provider
        // excluding expected situations such as Direct SUpervision, etc.
        void EnsureNoOverlapWithinCase()
        {

            var groups = Hours.GroupBy(x => x.CaseID);

            foreach (var group in groups)
            {

                foreach (var item in group)
                {

                    var checkList = group.Where(x =>
                    x.ID != item.ID
                    && x.Date == item.Date
                    && x.ServiceCode == "DR"
                    && item.ServiceCode == "DR").ToList();

                    var matches = checkList.Where(x =>
                        (x.TimeIn < item.TimeOut) &&
                        (x.TimeOut > item.TimeOut)
                        ).ToList();

                    matches.AddRange(checkList.Where(x =>
                        (x.TimeIn < item.TimeIn) && (x.TimeOut > item.TimeIn) ||
                        (x.TimeIn > item.TimeIn) && (x.TimeOut < item.TimeOut) ||
                        (x.TimeIn < item.TimeOut) && (x.TimeOut > item.TimeOut)
                        ).ToList());

                    matches = matches.Distinct().ToList();

                    foreach (var match in matches)
                    {
                        // create new validation error
                        AddError(ValidationErrors.CaseOverlap, item, match);
                    }


                }


            }


        }




        // make sure any DSU/SDR service entries have applicable counterpart within a single case
        void EnsureSupervisionOverlaps()
        {

            CheckSupervisionOverlapByDSU();
            CheckSupervisionOverlapBySDR();

        }

        void CheckSupervisionOverlapByDSU()
        {


            var groups = Hours.Where(x => x.ServiceCode == "DSU" || x.ServiceCode == "SDR").GroupBy(x => x.CaseID);

            foreach (var group in groups)
            {

                var dsuHours = group.Where(x => x.ServiceCode == "DSU");

                foreach (var dsuHour in dsuHours)
                {

                    var sdrHours = group.Where(x => x.ServiceCode == "SDR");

                    bool hasMatch = false;

                    foreach (var sdrHour in sdrHours)
                    {

                        if (sdrHour.TimeIn == dsuHour.TimeIn && sdrHour.TimeOut == dsuHour.TimeOut)
                        {
                            hasMatch = true;
                            break;
                        }

                    }

                    if (!hasMatch)
                    {
                        AddError(ValidationErrors.SupervisionMismatch, dsuHour, null);
                    }

                }

            }

        }

        void CheckSupervisionOverlapBySDR()
        {


            var groups = Hours.Where(x => x.ServiceCode == "DSU" || x.ServiceCode == "SDR").GroupBy(x => x.CaseID);

            foreach (var group in groups)
            {

                var sdrHours = group.Where(x => x.ServiceCode == "SDR");

                foreach (var sdrHour in sdrHours)
                {

                    var dsuHours = group.Where(x => x.ServiceCode == "DSU");

                    bool hasMatch = false;

                    foreach (var dsuHour in dsuHours)
                    {

                        if (sdrHour.TimeIn == dsuHour.TimeIn && sdrHour.TimeOut == dsuHour.TimeOut)
                        {
                            hasMatch = true;
                            break;
                        }

                    }

                    if (!hasMatch)
                    {
                        AddError(ValidationErrors.SupervisionMismatch, sdrHour, null);
                    }

                }

            }


            //var groups = Hours.GroupBy(x => x.CaseID);

            //foreach (var group in groups) {

            //    var items = group.Where(x => x.ServiceCode == "SDR");

            //    foreach (var item in items) {

            //        var checkedList = group.Where(x =>
            //            (x.ID != item.ID) &&
            //            (x.Date == item.Date) &&
            //            (x.ServiceCode == "DSU"));

            //        if (checkedList.Count() == 0) {
            //            addError(ValidationErrors.SupervisionMismatch, item, null);
            //        } else {
            //            if (checkedList.Count() > 1) {
            //                foreach (var cl in checkedList) {
            //                    addError(ValidationErrors.SupervisionMismatch, item, cl);
            //                }
            //            } else {

            //                // only one match, check times
            //                var x = checkedList.First();
            //                if (x.TimeIn != item.TimeIn && x.TimeOut != item.TimeOut) {
            //                    addError(ValidationErrors.SupervisionMismatch, item, x);
            //                }

            //            }
            //        }

            //    }


            //}

        }


        // make sure SSG entries have at least one other conterpart on some other case
        void EnsureSocialGroupOverlaps()
        {

            // TODO: ensure SSG overlaps (should be handled on input but check here for good measure

        }














        /***********************
         * 
         * TYPES
         * 
         **********************/

        public class Error
        {

            public ValidationErrors ErrorType { get; set; }
            public CaseAuthorizationHours MatchedOn { get; set; }
            public List<CaseAuthorizationHours> MatchedTo { get; set; } = new List<CaseAuthorizationHours>();

        }


    }
}
