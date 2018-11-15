using AABC.Data.Services;
using AABC.DomainServices.HoursResolution;
using AABC.ProviderPortal.App.Finalize.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.ProviderPortal.App.Finalize
{
    public class FinalizeService
    {


        //public List<PatientEx> GetPatientsExceedingMaxHoursPerDay(DateTime period) {
        //    PatientService svcPatient = new PatientService(_context);
        //    return svcPatient.GetPatientsExceedingMaxHoursPerDay(period);
        //}

        public List<ValidationItem> GetValidationFailures(int caseID, DateTime period, int providerID, bool includeHasHoursCheck = true)
        {
            var items = new List<ValidationItem>();
            //var hours = _caseService.GetCaseHoursByCaseByProvider(caseID, providerID).Where(h => h.Date >= new DateTime(period.Year, period.Month, 1) && h.Date < new DateTime(period.Year, period.Month, 1).AddMonths(1));
            var start = new DateTime(period.Year, period.Month, 1);
            var end = start.AddMonths(1);
            var hours = _context.Hours.Where(m => m.CaseID == caseID && m.ProviderID == providerID && m.Date >= start && m.Date < end).ToList();
            foreach (var he in hours)
            {
                var result = _hoursNotesValidator.Validate(he, EntryApp.ProviderApp, true);
                if (!result.IsValid)
                {
                    items.Add(new ValidationItem()
                    {
                        SourceDate = he.Date,
                        SourcePatientName = he.Case.Patient.CommonName,
                        SourceProviderName = he.Provider.FirstName + " " + he.Provider.LastName,
                        SourceServiceCode = he.Service.Code,
                        SourceTimeIn = he.Date + he.StartTime,
                        SourceTimeOut = he.Date + he.EndTime,
                        TypeValue = DomainServices.Hours.CrossHoursValidation.ValidationErrors.NotesMissing,
                    });
                }
            }

            if (includeHasHoursCheck)
            {
                var hoursNoCatalyst = hours.Where(h => !h.HasCatalystData).ToList();
                if (hoursNoCatalyst.Any())
                {
                    var sourcePatient = _context.Cases.Find(caseID).Patient;

                    hoursNoCatalyst.ForEach(nc => items
                        .Add(new ValidationItem()
                        {
                            SourceDate = nc.Date,
                            SourcePatientName = sourcePatient.CommonName,
                            SourceProviderName = nc.Provider.FirstName + " " + nc.Provider.LastName,
                            SourceServiceCode = nc.Service.Code,
                            SourceTimeIn = nc.Date + nc.StartTime,
                            SourceTimeOut = nc.Date + nc.EndTime,
                            TypeValue = DomainServices.Hours.CrossHoursValidation.ValidationErrors.NoCatalystData,
                        }));
                }
            }



            var validator = new DomainServices.Hours.CrossHoursValidation(_caseService, new DateTime(period.Year, period.Month, 1));
            validator.Validate();


            foreach (var err in validator.Errors)
            {

                bool skipProvider = true;

                // only validate POS, CO and SM entries
                if (err.ErrorType == DomainServices.Hours.CrossHoursValidation.ValidationErrors.CaseOverlap
                    || err.ErrorType == DomainServices.Hours.CrossHoursValidation.ValidationErrors.ProviderOverlapSelf)
                {

                    //if (err.ErrorType != DomainServices.Hours.CrossHoursValidation.ValidationErrors.AideMaxHoursPerDayPerAide) {

                    System.Diagnostics.Debug.WriteLine(err.MatchedOn.ServiceCode);

                    // ignore SSG cases
                    if (err.MatchedOn.ServiceCode != "SSG")
                    {

                        if (err.MatchedOn.ProviderID == providerID && err.MatchedOn.CaseID == caseID)
                        {
                            skipProvider = false;
                        }
                        else
                        {
                            if (err.MatchedTo != null && err.MatchedTo.Count > 0)
                            {
                                if (err.MatchedTo[0].ProviderID == providerID && err.MatchedTo[0].CaseID == caseID)
                                {
                                    skipProvider = false;
                                }
                            }
                        }

                    }

                }





                if (!skipProvider)
                {

                    var item = new ValidationItem();

                    var sourcePatient = _context.Cases.Find(err.MatchedOn.CaseID.Value).Patient;
                    var sourceProvider = _context.Providers.Find(err.MatchedOn.ProviderID);

                    item.SourceDate = err.MatchedOn.Date;
                    item.SourcePatientName = sourcePatient.CommonName;
                    item.SourceProviderName = sourceProvider.FirstName + " " + sourceProvider.LastName;
                    item.SourceServiceCode = err.MatchedOn.ServiceCode;
                    item.SourceTimeIn = err.MatchedOn.TimeIn;
                    item.SourceTimeOut = err.MatchedOn.TimeOut;

                    item.TypeValue = err.ErrorType;

                    if (err.MatchedTo != null && err.MatchedTo.Count > 0)
                    {

                        var m = err.MatchedTo[0];

                        var patient = _context.Cases.Find(m.CaseID.Value).Patient;
                        var provider = _context.Providers.Find(m.ProviderID);

                        item.PartnerDate = m.Date;
                        item.PartnerPatientName = patient.CommonName;
                        item.PartnerProviderName = provider.FirstName + " " + provider.LastName;
                        item.PartnerServiceCode = m.ServiceCode;
                        item.PartnerTimeIn = m.TimeIn;
                        item.PartnerTimeOut = m.TimeOut;
                    }

                    items.Add(item);
                }
            }

            return items;
        }








        private Data.V2.CoreContext _context;
        private readonly HoursNotesValidator _hoursNotesValidator;
        private readonly CaseService _caseService;

        public FinalizeService()
        {
            _context = AppService.Current.Context;
            _hoursNotesValidator = new HoursNotesValidator();
            _caseService = new CaseService();
        }

    }
}