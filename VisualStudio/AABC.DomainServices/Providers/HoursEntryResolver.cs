using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ValidationHelper = Dymeng.Framework.Validation;

namespace AABC.DomainServices.Providers
{

    /// <summary>
    /// Validates and Resolves a new CaseAuthorizationHours entry
    /// </summary>
    public class HoursEntryResolver
    {

        private const string ASSESSMENT_CODE = "I-ASS";
        private const string FOLLOWUP_CODE = "FUA";
        private const int BCBA_TYPE = 15;

        /*************************
        * 
        * ENUMS
        * 
        ************************/

        /// <summary>
        /// Custom IDs for ValidationErrors
        /// </summary>
        public enum HoursEntryErrorID
        {
            DateIsFutureDate = 0,               // if the date specified isn't valid
            TimeInOrOutNotValid = 1,            // if the time in is greater than the time out (or other generic issue)
            ServiceIsNull = 2,
            NotesAreNull = 3,
            ServiceToAuthClassMapFailure = 4,   // if an AuthClass can't be found based on the specified service
            ServiceToAuthMapFailure = 5,        // if a CaseAuthorization can't be found based on the specified Service/AuthClass
            ServiceMapsToMultipleAuths = 6,
            PerProviderTimeOverlap = 7,         // the provider has entered an overlapping hours set (of their own previously entered hours)
            ProviderMoreThan5HoursPerDay = 8,
            CaseMoreThan5HoursPerDay = 9,
            AideDROverlap = 10,
            BCBAServiceOverTwoHours = 11,       // BCBA entered more than two hours for a service (excepting Assessments)
            BCBAAssessmentOverSixHours = 12,    // BCBA entered more than 6 hours this day for an Assessment
            Aide4HoursPerCasePerService = 13    // Aide entered more than 4 hours for this service on this case on this date
        }

        public enum ValidationResultStatus
        {
            None = 0,
            ProviderPending = 1,
            ProviderCommited = 2,
            ProviderFinalized = 3,
            OfficeScrubbed = 4,
            OfficeFinalized = 5
        }

        

        /*************************
        * 
        * FIELDS
        * 
        ************************/

        const string DEFAULT_AUTH_CLASS_CODE = "GENERAL";
        const int DEFAULT_AUTH_CLASS_ID = 3;    // last resort, if can't be found using code

        private Data.Services.ICaseService caseService;
        
        Domain.Cases.Case targetCase { get; set; }
        Domain.Cases.CaseAuthorizationHours subjectHours { get; set; }

        Data.V2.CoreContext _context2;


        /*************************
        * 
        * PROPERTIES
        * 
        ************************/

        /// <summary>
        /// After running Resolve(), the highest status allowed per the validation analysis
        /// </summary>
        public ValidationResultStatus PassingStatus { get; private set; } = ValidationResultStatus.None;

        public bool IgnoreNotesCheck { get; set; } = false;

        /// <summary>
        /// Returns true if any ValidationErrors were found during resolution
        /// </summary>
        public bool HasValidationErrors { get { return ValidationErrors.Count == 0 ? false : true; } }
        
        /// <summary>
        /// List of ValidationErrors as applicable.  Message will be filled, Exception may or may not be.
        /// </summary>
        public List<Dymeng.Framework.Validation.ValidationError> ValidationErrors { get; private set; }

        /// <summary>
        /// List of ValidationErrors that will be show to the Provider Portal UI.  Message will be filled, Exception may or may not be.
        /// </summary>
        public List<Dymeng.Framework.Validation.ValidationError> UserVisibleValidationErrors {
            get
            {
                return ValidationErrors.Where(x => 
                    x.ID != (int)HoursEntryErrorID.ServiceMapsToMultipleAuths &&
                    x.ID != (int)HoursEntryErrorID.ServiceToAuthClassMapFailure &&
                    x.ID != (int)HoursEntryErrorID.ServiceToAuthMapFailure
                ).ToList();
            }
        }
        

        /*************************
        * 
        * CONSTRUCTOR
        * 
        ************************/

        /// <summary>
        /// Accepts a Case and a proposed AuthorizationHours object, validates and resolves AuthorizationHours based on the Case
        /// </summary>
        /// <param name="targetCase">Expects Domain.Cases.Case object mapped with CaseProviders, Authorizations and AuthorizationHours</param>
        /// <param name="authHours">The AuthorizationHours instance to be resolved (not yet part of Cases.Authorizations.Hours).  Provider, Service and ProviderType are required mappings.</param>
        public HoursEntryResolver(Domain.Cases.Case targetCase, Domain.Cases.CaseAuthorizationHours authHours) {

            this.targetCase = targetCase;
            subjectHours = authHours;
            ValidationErrors = new List<Dymeng.Framework.Validation.ValidationError>();
            
            caseService = new Data.Services.CaseService();
            _context2 = new Data.V2.CoreContext();

        }

        public HoursEntryResolver(Domain.Cases.Case targetCase, Domain.Cases.CaseAuthorizationHours authHours, Data.Services.ICaseService caseService) {
            this.targetCase = targetCase;
            subjectHours = authHours;
            ValidationErrors = new List<Dymeng.Framework.Validation.ValidationError>();
            this.caseService = caseService;
            _context2 = new Data.V2.CoreContext();
        }




        /*************************
        * 
        * PUBLIC METHODS
        * 
        ************************/

        /// <summary>
        /// Validates and resolves the CaseAuthorizationHours instance.  Check validation properties after running for results.
        /// </summary>
        /// <returns>Resvolved CaseAuthorizationHours instance</returns>
        public Domain.Cases.CaseAuthorizationHours Resolve() {

            resolveBasicInput();

            
            if (!validateBasicInput()) {
                PassingStatus = ValidationResultStatus.None;
                return null;
            } else {
                PassingStatus = ValidationResultStatus.ProviderFinalized;
            }

            resolveAuthMapping();

            // now we should have a mapped authorization on file
            // see if it collides with any of the rules for how hours can be applied

            //validateProviderMax5HoursPerDay();    //remove per request
            //validateCaseMax5HoursPerDay();        // removed per request, replaced with 2hr BCBA services (except assessment) and max aide hours 4 per case
            
            validateNotSameProviderTimeOverlap();
            validateNoAideDROverlap();

            validate4AideHoursPerCasePerDay();
            validate2BCBAHoursPerEntryExceptAssessment();
            validate6AssessementHoursPerCasePerDay();
            
            calculateFinalValidations();

            return subjectHours;

        }





        /*************************
        * 
        * PRIVATE METHODS
        * 
        ************************/


        void validate4AideHoursPerCasePerDay() {
            
            if (subjectHours.Provider.Type.ID == BCBA_TYPE) {
                return;
            }

            if (subjectHours.HoursTotal > 4) {
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.Aide4HoursPerCasePerService,
                    "Unable to log more than four hours.", null);
                return;
            }

            var c = _context2.Cases.Find(targetCase.ID);

            var hours = c.Hours
                .Where(x => 
                    x.Provider.ProviderTypeID != BCBA_TYPE 
                    && x.ServiceID == subjectHours.Service.ID 
                    && x.Date == subjectHours.Date)
                .ToList();

            // if we have an ID on the subject hours, it's previously existing, so
            // we should back it out of the tested collection
            if (subjectHours.ID.HasValue) {
                var previouslyExistingHoursRecord = hours.Where(x => x.ID == subjectHours.ID).FirstOrDefault();
                if (previouslyExistingHoursRecord != null) {
                    hours.Remove(previouslyExistingHoursRecord);
                }
            }

            var totalHours = hours.Sum(x => x.TotalHours);

            if (totalHours + (decimal)subjectHours.HoursTotal > 4) {
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.Aide4HoursPerCasePerService,
                    "Unable to log more than four hours.", null);
                return;
            }

        }

        void validate2BCBAHoursPerEntryExceptAssessment() {

            if (subjectHours.Provider.Type.ID != BCBA_TYPE) {
                return;
            }

            if (isBcbaAssessment()) {
                return;
            }

            if (subjectHours.HoursTotal > 6) {

                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.BCBAServiceOverTwoHours,
                    "Unable to log more than two hours per service, except Assessments.", null);
                return;
            }

            // get all of their other hours of this service type for the case
            var c = _context2.Cases.Find(targetCase.ID);

            var hours = c.Hours.Where(x => x.ServiceID == subjectHours.Service.ID && x.Date == subjectHours.Date).ToList();

            // if we have an ID on the subject hours, it's previously existing, so
            // we should back it out of the tested collection
            if (subjectHours.ID.HasValue) {
                var previouslyExistingHoursRecord = hours.Where(x => x.ID == subjectHours.ID).FirstOrDefault();
                if (previouslyExistingHoursRecord != null) {
                    hours.Remove(previouslyExistingHoursRecord);
                }
            }

            var totalHours = hours.Sum(x => x.TotalHours);

            if (totalHours + (decimal)subjectHours.HoursTotal > 2) {
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.BCBAServiceOverTwoHours,
                    "Unable to log more than two hours per service, except Assessments.", null);
                return;
            }

        }


        private bool isBcbaAssessment() {
            if (subjectHours.Provider.Type.ID != BCBA_TYPE) {
                return false;
            }
            if (subjectHours.ServiceCode == ASSESSMENT_CODE) {
                return true;
            }
            if (subjectHours.ServiceCode == FOLLOWUP_CODE) {
                return true;
            }
            return false;
        }

        void validate6AssessementHoursPerCasePerDay() {
            
            if (subjectHours.Provider.Type.ID != BCBA_TYPE) {
                return;
            }

            if (!isBcbaAssessment()) {
                return;
            }

            if (subjectHours.HoursTotal > 6) {
                
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.BCBAAssessmentOverSixHours,
                    "Unable to log more than six hours per Assessment.", null);
                return;                
            }


            // get any other assessments for this case/day
            var c = _context2.Cases.Find(targetCase.ID);

            var hours = c.Hours.Where(
                x => (x.Service.Code == ASSESSMENT_CODE || x.Service.Code == FOLLOWUP_CODE)
                    && x.Date == subjectHours.Date
                ).ToList();

            // if we have an ID on the subject hours, it's previously existing, so
            // we should back it out of the tested collection
            if (subjectHours.ID.HasValue) {
                var previouslyExistingHoursRecord = hours.Where(x => x.ID == subjectHours.ID).FirstOrDefault();
                if (previouslyExistingHoursRecord != null) {
                    hours.Remove(previouslyExistingHoursRecord);
                }
            }

            var totalHours = hours.Sum(x => x.TotalHours);

            if (totalHours + (decimal)subjectHours.HoursTotal > 6) {
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.BCBAAssessmentOverSixHours,
                    "Unable to log more than six Assessment hours per day.", null);
                return;
            }

            
        }

        void validateNoAideDROverlap() {
            
            if (subjectHours.ServiceCode != "DR") {
                return;
            }

            var c = _context2.Cases.Find(targetCase.ID);

            var caseHours = c.Hours.Where(x => x.Date == subjectHours.Date && x.ID != subjectHours.ID).ToList();

            var overlappingHours = caseHours.Where(x =>
                (subjectHours.TimeOut.TimeOfDay > x.StartTime) &&
                (subjectHours.TimeIn.TimeOfDay < x.EndTime)).ToList();

            // if we have an ID on the subject hours, it's previously existing, so
            // we should back it out of the tested collection
            if (subjectHours.ID.HasValue) {
                var previouslyExistingHoursRecord = overlappingHours.Where(x => x.ID == subjectHours.ID).FirstOrDefault();
                if (previouslyExistingHoursRecord != null) {
                    overlappingHours.Remove(previouslyExistingHoursRecord);
                }
            }

            foreach (var h in overlappingHours) {
                if (h.Service.Code == "DR") {
                    ValidationHelper.ValidationError.AddError(
                        ValidationErrors,
                        (int)HoursEntryErrorID.AideDROverlap,
                        "A DR Service code has already been logged for these hours.", null);
                }
            }


            //if ((subjectHours.TimeOut > eh.TimeIn) && (subjectHours.TimeIn < eh.TimeOut)) {

        }


        void validateCaseMax5HoursPerDay() {
            
            var c = _context2.Cases.Find(targetCase.ID);

            var caseHours = c.Hours.Where(x => x.Date == subjectHours.Date).ToList();

            // if we have an ID on the subject hours, it's previously existing, so
            // we should back it out of the tested collection
            decimal existingHoursOffset = 0;
            if (subjectHours.ID.HasValue) {
                var previouslyExistingHoursRecord = caseHours.Where(x => x.ID == subjectHours.ID).FirstOrDefault();
                if (previouslyExistingHoursRecord != null) {
                    existingHoursOffset = previouslyExistingHoursRecord.TotalHours;
                }
            }

            var totalHours = caseHours.Sum(x => x.TotalHours);
            totalHours = totalHours - existingHoursOffset;
            totalHours = totalHours + (decimal)subjectHours.HoursTotal;

            if (totalHours > 5 && !HasBCBAAssessmentHours(subjectHours, caseHours))
            {
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.CaseMoreThan5HoursPerDay,
                    "Case cannot have more than 5 hours per day logged.", null);
            }

            if (totalHours > 6 && HasBCBAAssessmentHours(subjectHours, caseHours))
            {
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.CaseMoreThan5HoursPerDay,
                    "Case cannot have more than 6 hours per day logged.", null);
            }

        }

        private static bool HasBCBAAssessmentHours(Domain.Cases.CaseAuthorizationHours newHours, List<Domain2.Hours.Hours> existingHours)
        {
            if((newHours.ServiceCode == ASSESSMENT_CODE || newHours.ServiceCode == FOLLOWUP_CODE)
                && newHours.Provider.Type.ID == BCBA_TYPE)
            {
                return true;
            }

            if (existingHours.Where(x =>
                 (x.Service.Code == ASSESSMENT_CODE || x.Service.Code == FOLLOWUP_CODE)
                 && x.Provider.ProviderTypeID == BCBA_TYPE).Any())
            {
                return true;
            }

            return false;
        }



        void calculateFinalValidations() {
            
            // this is pre-inialized at validateBasicInput test in Resolve()

            foreach (var ve in ValidationErrors) {

                if (ve.ID == (int)HoursEntryErrorID.PerProviderTimeOverlap) {
                    PassingStatus = ValidationResultStatus.None;
                }   

                if (ve.ID == (int)HoursEntryErrorID.CaseMoreThan5HoursPerDay) {
                    PassingStatus = ValidationResultStatus.None;
                }

                if (ve.ID == (int)HoursEntryErrorID.AideDROverlap)
                {
                    PassingStatus = ValidationResultStatus.None;
                }

                if (ve.ID == (int)HoursEntryErrorID.Aide4HoursPerCasePerService) {
                    PassingStatus = ValidationResultStatus.None;
                }

                if (ve.ID == (int)HoursEntryErrorID.BCBAAssessmentOverSixHours) {
                    PassingStatus = ValidationResultStatus.None;
                }

                if (ve.ID == (int)HoursEntryErrorID.BCBAServiceOverTwoHours) {
                    PassingStatus = ValidationResultStatus.None;
                }
                
                // other checks here

            }
        }

        void validateNotSameProviderTimeOverlap() {

            try {

                List<Domain.Cases.CaseAuthorizationHours> existingHours = caseService.GetCaseHoursByCase(targetCase.ID.Value);
                if (existingHours == null || existingHours.Count == 0) {
                    return;
                }

                var testList = existingHours.Where(x => x.Date == subjectHours.Date && x.ProviderID == subjectHours.Provider.ID).ToList();

                // if this is an existing hours entry (it'll have an ID), we need to make sure
                // this hard pull from the db didn't include this instance
                if (subjectHours.ID.HasValue) {
                    var toRemove = testList.Where(x => x.ID.Value == subjectHours.ID.Value).FirstOrDefault();
                    if (toRemove != null) {
                        testList.Remove(toRemove);
                    }
                }

                foreach (var eh in testList) {

                    // TEST ME
                    if ((subjectHours.TimeOut > eh.TimeIn) && (subjectHours.TimeIn < eh.TimeOut)) {

                        ValidationHelper.ValidationError.AddError(
                            ValidationErrors,
                            (int)HoursEntryErrorID.PerProviderTimeOverlap,
                            "Provider cannot have overlapping hours on the same day.", null);
                    }

                }

            } catch (Exception e) {
                Dymeng.Framework.Exceptions.LogMessageToTelementry(e.ToString(), null);
            }

            
            
        }


        void validateProviderMax5HoursPerDay() {

            // removed per request 2017-02-01, apply to all providers, not just aides
            //if (subjectHours.Provider.Type.Code != "AIDE") {
            //    return;
            //}

            var date = subjectHours.Date;
            var providerID = subjectHours.Provider.ID;

            var context = new Data.Models.CoreEntityModel();

            var h = context.CaseAuthHours
                .Where(x =>
                    x.HoursDate == date
                    && x.CaseProviderID == providerID
                    && x.CaseID == subjectHours.CaseID)
                .GroupBy(x => x.CaseProviderID)
                .Select(x => new {
                    ProviderID = x.Key,
                    TotalHours = x.Sum(y => y.HoursTotal)
                }).ToList();

            var previousHours = h.Sum(x => x.TotalHours);
            

            //var z = h.Where(x => x.TotalHours + (decimal)subjectHours.HoursTotal > 5).ToList();

            //if (z.Count > 0) {
            if ((previousHours + (decimal)subjectHours.HoursTotal) > 5) { 
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.ProviderMoreThan5HoursPerDay,
                    "Cannot log more than 5 hours per day.", null);
            }
                
        }


        /// <summary>
        /// Validates the basic required inputs.  Fills ValidationErrors as applicable.
        /// </summary>
        bool validateBasicInput() {

            bool valid = true;

            if (subjectHours.Date > DateTime.Now.Date) {
                valid = false;
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.DateIsFutureDate,
                    "Date must not be in the future", null);
            }

            if (subjectHours.TimeIn >= subjectHours.TimeOut) {
                valid = false;
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.TimeInOrOutNotValid,
                    "Time In must be later than Time Out", null);
            }

            if (subjectHours.Service == null) {
                valid = false;
                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.ServiceIsNull,
                    "Service must be specified", null);
            }

            if (!IgnoreNotesCheck) {
                if (subjectHours.Notes == null || subjectHours.Notes == "") {
                    valid = false;
                    ValidationHelper.ValidationError.AddError(
                        ValidationErrors, (int)HoursEntryErrorID.NotesAreNull, "Notes must be entered", null);
                }
            }
                        
            return valid;
        }

        /// <summary>
        /// Resolves the basic inputs, normalizes dates and times
        /// </summary>
        void resolveBasicInput() {
            // normalize the date
            subjectHours.Date = subjectHours.Date.Date;

            // normalize the times
            subjectHours.TimeIn = subjectHours.Date + subjectHours.TimeIn.TimeOfDay;
            subjectHours.TimeOut = subjectHours.Date + subjectHours.TimeOut.TimeOfDay;
            subjectHours.HoursTotal = (subjectHours.TimeOut - subjectHours.TimeIn).TotalHours;
            
            subjectHours.PayableHours = Math.Round(subjectHours.HoursTotal * 4, MidpointRounding.ToEven) / 4;
            subjectHours.BillableHours = Math.Round(subjectHours.HoursTotal * 4, MidpointRounding.ToEven) / 4;

            subjectHours.CaseID = targetCase.ID.Value;
        }

        /// <summary>
        /// Resolves the mappings to CaseAuthorizationClass and CaseAuthorization
        /// </summary>
        void resolveAuthMapping() {

            int providerTypeID = subjectHours.Provider.Type.ID.Value;
            int serviceID = subjectHours.Service.ID.Value;

            // determine the auth class we need to map to
            int? associatedAuthClassID = getAssociatedCaseAuthClassID();
            if (associatedAuthClassID == null) {

                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.ServiceToAuthClassMapFailure,
                    "Unable to find explicit Auth Class map based on Provider Type and Service", null);

                Domain.Cases.AuthorizationClass ac = caseService.GetAuthClassByCode(DEFAULT_AUTH_CLASS_CODE);
                if (ac != null) {
                    associatedAuthClassID = ac.ID.Value;
                } else {
                    associatedAuthClassID = DEFAULT_AUTH_CLASS_ID;
                }

            }


            // determine the authorization we need to map to
            Domain.Cases.Authorization mappedAuthorization = null;
            var classedAuths = targetCase.Authorizations.Where(x => x.AuthClass.ID == associatedAuthClassID).ToList();

            if (classedAuths == null || classedAuths.Count == 0) {

                ValidationHelper.ValidationError.AddError(
                    ValidationErrors,
                    (int)HoursEntryErrorID.ServiceToAuthMapFailure,
                    "Unable to find explicit Authorization to map based on Service", null);

            } else {

                // find the authorization whose start/end dates are within range
                var applicableAuths = classedAuths.Where(x => x.StartDate <= subjectHours.Date && x.EndDate.Date.AddDays(1) > subjectHours.Date).ToList();

                if (applicableAuths == null || applicableAuths.Count == 0) {
                    ValidationHelper.ValidationError.AddError(
                        ValidationErrors,
                        (int)HoursEntryErrorID.ServiceToAuthMapFailure,
                        "Unable to find explicit Authorization to map based on Service", null);
                } else {

                    if (applicableAuths.Count > 1) {
                        ValidationHelper.ValidationError.AddError(
                            ValidationErrors,
                            (int)HoursEntryErrorID.ServiceMapsToMultipleAuths,
                            "The specified Service maps to multiple possible Authorizations", null);
                    } else {

                        mappedAuthorization = applicableAuths[0];

                    }

                }
            }
            
            subjectHours.Authorization = mappedAuthorization as Domain.Cases.CaseAuthorization;

        }



        


        private int? getAssociatedCaseAuthClassID() {
            
            return caseService.GetAssociatedCaseAuthID(subjectHours.Provider.Type.ID.Value, subjectHours.Service.ID.Value);

        }


        /*************************
        * 
        * EVENTS (SUBSCRIBED)
        * 
        ************************/


        /*************************
        * 
        * EVENTS (PUBLISHED)
        * 
        ************************/


        /*************************
        * 
        * SUBCLASSES
        * 
        ************************/
        
    }
}
