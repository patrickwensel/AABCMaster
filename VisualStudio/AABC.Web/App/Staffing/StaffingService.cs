using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Domain2.Providers;
using AABC.Web.App.Staffing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Staffing
{
    public class StaffingService
    {

        public IEnumerable<StaffingListItemVM> GetStaffingLogListItems()
        {
            var items = Context.Database.SqlQuery<StaffingListItemVM>("EXEC dbo.GetStaffingLogSummary").ToList();
            return items;            
        }


        public StaffingLogVM CreateStaffingLogVM(int caseId, StaffingLog staffingLog)
        {
            StaffingLogVM model = null;
            var patient = Context.Patients.SingleOrDefault(m => m.Cases.Any(c => c.ID == caseId));
            if (patient != null)
            {
                var @case = Context.Cases.SingleOrDefault(m => m.ID == caseId);
                var zipInfoRepository = new ZipInfoRepository();
                var caseService = new Data.Services.CaseService();
                var guardianRelationships = caseService.GetGuardianRelationships();
                model = new StaffingLogVM
                {
                    PatientID = patient.ID,
                    CaseID = caseId,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    DateOfBirth = patient.DateOfBirth,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    Address1 = patient.Address1,
                    Address2 = patient.Address2,
                    City = patient.City,
                    State = patient.State,
                    Zip = patient.Zip,
                    County = zipInfoRepository.GetCounty(patient.Zip),
                    Guardians = new List<GuardianInfoVM> {
                    new GuardianInfoVM {
                        FirstName = patient.GuardianFirstName,
                        LastName = patient.GuardianLastName,
                        Relationship = GetGuardianRelationship(patient.GuardianRelationshipID, guardianRelationships),
                        Email = patient.GuardianEmail,
                        CellPhone = patient.GuardianCellPhone,
                        HomePhone = patient.GuardianHomePhone,
                        WorkPhone = patient.GuardianWorkPhone,
                        Notes = patient.GuardianNotes
                    },
                    new GuardianInfoVM            {
                        FirstName = patient.Guardian2FirstName,
                        LastName = patient.Guardian2LastName,
                        Relationship = GetGuardianRelationship(patient.Guardian2RelationshipID, guardianRelationships),
                        Email = patient.Guardian2Email,
                        CellPhone = patient.Guardian2CellPhone,
                        HomePhone = patient.Guardian2HomePhone,
                        WorkPhone = patient.Guardian2WorkPhone,
                        Notes = patient.Guardian2Notes
                    },
                    new GuardianInfoVM
                    {
                        FirstName = patient.Guardian3FirstName,
                        LastName = patient.Guardian3LastName,
                        Relationship = GetGuardianRelationship(patient.Guardian3RelationshipID, guardianRelationships),
                        Email = patient.Guardian3Email,
                        CellPhone = patient.Guardian3CellPhone,
                        HomePhone = patient.Guardian3HomePhone,
                        WorkPhone = patient.Guardian3WorkPhone,
                        Notes = patient.Guardian3Notes
                    }
                },
                    FunctioningLevel = @case.FunctioningLevel?.Name,
                    Notes = patient.Notes,
                    CaseProviders = Context.CaseProviders
                    .Where(m => m.CaseID == caseId)
                    .ToList()
                    .Select(m => new CaseProviderVM
                    {
                        ProviderID = m.Provider.ID,
                        ProviderLastName = m.Provider.LastName,
                        ProviderFirstName = m.Provider.FirstName,
                        ProviderType = m.Provider.GetProviderTypeFullCode(),
                        IsBCBA = m.Provider.IsBCBA,
                        IsActive = CaseIsActive(DateTime.Now, m.StartDate, m.EndDate)
                    })
                .OrderByDescending(m => m.IsActive)
                .ThenByDescending(m => m.IsBCBA)
                .ThenBy(m => m.ProviderLastName)
                .ThenBy(m => m.ProviderFirstName),
                    SystemSpecialAttentionNeeds = Context.SpecialAttentionNeeds
                    .Where(n => n.Active)
                    .Select(n => new SpecialAttentionNeedVM
                    {
                        ID = n.ID,
                        Code = n.Code,
                        Name = n.Name
                    })
                    .ToList(),
                    SpecialAttentionNeedIDs = staffingLog.SpecialAttentionNeeds != null ? staffingLog.SpecialAttentionNeeds.Select(n => n.ID).ToList() : new List<int>(),
                    ParentalRestaffRequest = staffingLog.ParentalRestaffRequest,
                    HoursOfABATherapy = staffingLog.HoursOfABATherapy,
                    AidesRespondingNo = staffingLog.AidesRespondingNo,
                    AidesRespondingMaybe = staffingLog.AidesRespondingMaybe,
                    ScheduleRequest = ScheduleRequestVM.FromInt(staffingLog.ScheduleRequest),
                    DateWentToRestaff = staffingLog.DateWentToRestaff,
                    ProviderGenderPreference = string.IsNullOrEmpty(staffingLog.ProviderGenderPreference) ? '0' : staffingLog.ProviderGenderPreference[0]
                };
            }
            return model;
        }


        public AddProviderContactLogVM CreateProviderContactLogVM(int caseId, ProviderTypeIDs type)
        {
            var typeID = (int)type;
            var providers = Context.StaffingLogProviders
                .Where(m => m.StaffingLogID == caseId && m.Provider.ProviderTypeID == typeID)
                .Select(m => new CaseProviderVM
                {
                    ProviderID = m.Provider.ID,
                    ProviderLastName = m.Provider.LastName,
                    ProviderFirstName = m.Provider.FirstName
                })
                .OrderBy(m => m.ProviderFirstName)
                .ToList();

            var statuses = Context.StaffingLogProviderStatuses.Where(s => s.Active).ToList();
            var model = new AddProviderContactLogVM
            {
                CaseId = caseId,
                Statuses = statuses,
                Providers = providers
            };

            return model;
        }


        public IEnumerable<ProviderContactLogListItemVM> LoadProviderContactLogVM(int caseId, ProviderTypeIDs type)
        {
            var typeID = (int)type;
            var providers = Context.StaffingLogProviderContactLog
                .Where(l => l.StaffingLogProvider.StaffingLogID == caseId && l.StaffingLogProvider.Provider.ProviderTypeID == typeID)
                .OrderByDescending(l => l.ContactDate)
                .ToList()
                .Select(l => new ProviderContactLogListItemVM
                {
                    ContactDate = l.ContactDate,
                    FollowUpDate = l.FollowUpDate,
                    Notes = l.Notes,
                    Status = l.Status.StatusName,
                    ProviderName = l.StaffingLogProvider.Provider.FirstName + " " + l.StaffingLogProvider.Provider.LastName,
                    ProviderTypeCode = l.StaffingLogProvider.Provider.GetProviderTypeFullCode()
                });

            return providers;
        }


        public void SaveProviderContactLog(AddProviderContactLogVM model)
        {
            var staffingProvider = Context.StaffingLogProviders
                .FirstOrDefault(p => p.StaffingLogID == model.CaseId && p.ProviderID == model.ProviderId);

            var log = new StaffingLogProviderContactLog
            {
                ContactDate = model.ContactDate,
                StatusID = model.StatusId.Value,
                Notes = model.Notes,
                FollowUpDate = model.FollowUpDate,
                StaffingLogProviderID = staffingProvider.ID,
                DateCreated = DateTime.UtcNow,
                CreatedByUserID = AppService.Current.UserID
            };

            Context.StaffingLogProviderContactLog.Add(log);
            Context.SaveChanges();
        }


        public List<ParentContactLogListItemVM> LoadParentContactLogVM(int caseId)
        {
            var caseService = new Data.Services.CaseService();
            var relationships = caseService.GetGuardianRelationships().ToDictionary(k => k.ID, v => v.Name);

            var log = Context.StaffingLogParentContactLog
                .Where(l => l.StaffingLogID == caseId)
                .OrderByDescending(l => l.ContactDate)
                .ToList()
                .Select(l => new ParentContactLogListItemVM
                {
                    ContactDate = l.ContactDate,
                    Notes = l.Notes,
                    ContactedPerson = relationships[l.GuardianRelationshipID] +
                                      (!string.IsNullOrWhiteSpace(l.ContactedPersonName)
                                          ? " (" + l.ContactedPersonName + ")"
                                          : ""),
                    ContactMethod = l.ContactMethodType.ToString() + " (" + l.ContactMethodValue + ")"
                })
                .ToList();

            return log;
        }


        public AddParentContactLogVM CreateParentContactLogVM(int caseId)
        {
            var caseService = new Data.Services.CaseService();
            var relationships = caseService.GetGuardianRelationships();

            var model = new AddParentContactLogVM
            {
                CaseId = caseId,
                Relationships = relationships
            };

            return model;
        }


        public void SaveParentContactLog(AddParentContactLogVM model)
        {
            var log = new StaffingLogParentContactLog
            {
                ContactDate = model.ContactDate,
                StaffingLogID = model.CaseId,
                Notes = model.Notes,
                DateCreated = DateTime.UtcNow,
                CreatedByUserID = AppService.Current.UserID,
                ContactMethodType = model.ContactMethodType,
                ContactMethodValue = model.ContactMethodValue,
                GuardianRelationshipID = model.GuardianRelationshipId,
                ContactedPersonName = model.ContactedPersonName
            };
            Context.StaffingLogParentContactLog.Add(log);
            Context.SaveChanges();
        }


        public IEnumerable<Provider> GetProviders(IEnumerable<int> ids)
        {
            return Context.Providers.Where(r => ids.Contains(r.ID)).ToList();
        }


        public void LogEmailSent(IEnumerable<int> ids, string text)
        {
            var providers = GetProviders(ids);
            foreach (var p in providers)
            {
                p.Notes = ($"{DateTime.UtcNow.ToString("MM-dd-yy")} {text}" + Environment.NewLine + p.Notes).TrimEnd(Environment.NewLine.ToCharArray());
            }
            Context.SaveChanges();
        }


        private string GetGuardianRelationship(int? id, List<Domain.General.GuardianRelationship> items)
        {
            if (!id.HasValue)
            {
                return null;
            }
            else
            {
                return items.Where(x => x.ID == id.Value).FirstOrDefault()?.Name;
            }
        }


        private static bool CaseIsActive(DateTime date, DateTime? startDate, DateTime? endDate)
        {
            var s = startDate ?? date;
            var e = endDate ?? date;
            return date.Date >= s.Date && date.Date <= e.Date;
        }


        private readonly CoreContext Context;
        private readonly StaffingRepository StaffingLogRepository;


        public StaffingService()
        {
            Context = AppService.Current.DataContextV2;
            StaffingLogRepository = new StaffingRepository();
        }

    }
}
