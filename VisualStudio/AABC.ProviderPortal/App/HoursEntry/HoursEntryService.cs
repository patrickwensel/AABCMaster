using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.DomainServices.Sessions;
using AABC.Shared.Web.App.HoursEntry;
using AABC.Shared.Web.App.HoursEntry.Models;
using System.Linq;

namespace AABC.ProviderPortal.App.HoursEntry
{
    public class HoursEntryService : HoursEntryServiceBase
    {

        public HoursEntryService(CoreContext context) : base(context, HoursEntryMode.ProviderEntry) { }

        public HoursEntryVM GetCatalsytHoursVM(int entryID)
        {
            var entry = Context.CatalystPreloadEntries.Find(entryID);
            var provider = Context.Providers.Find(entry.ProviderID);
            var c = Context.Cases.Find(entry.CaseID);
            var insurance = c.GetActiveInsuranceAtDate(entry.Date);
            var model = new HoursEntryVM
            {
                IsOnAideLegacyMode = SessionReportService.IsOnAideLegacyMode(provider),
                IsEditable = true,
                Status = (int)Domain2.Hours.HoursStatus.Pending,
                HasData = true,
                CatalystPreloadID = entryID,
                IsAdminMode = false,
                Date = entry.Date,
                EntryID = null,
                ProviderID = entry.ProviderID,
                ProviderTypeID = provider.ProviderTypeID,
                ProviderTypeCode = provider.ProviderType.Code,
                PatientID = c.PatientID,
                PatientName = c.Patient.CommonName,
                InsuranceID = insurance?.ID,
                InsuranceName = insurance?.Insurance.Name ?? "Select a date to determine the active insurance",
                TimeIn = null,
                TimeOut = null,
                Note = entry.Notes,
                ServiceID = null,
                ServiceLocations = GetServiceLocations(),
                IsTrainingEntry = false,
                Services = GetHoursEntryServices((Domain2.Providers.ProviderTypeIDs)provider.ProviderTypeID),
                ActivePatients = GetHoursEntryActivePatients(entry.ProviderID, entry.Date),
                NoteGroups = GetNewHoursEntryNoteGroups(provider.ProviderTypeID)
            };
            model.ServiceLocationID = model.ServiceLocations.Where(x => x.Name == "Home").FirstOrDefault()?.ID;
            return model;
        }

    }
}