using AABC.Data.V2;
using AABC.Domain2.Notes;
using AABC.Domain2.Referrals;
using AABC.DomainServices.Referrals;
using AABC.Web.App.Patients;
using AABC.Web.App.Referrals.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.Referrals
{
    public class ReferralService : IReferralService<Referral, Domain2.Staff.Staff, Domain2.Infrastructure.Language, ReferralDismissalReason, ReferralSourceType>
    {
        private readonly CoreContext Context;
        private readonly ReferralAcceptanceService ReferralAcceptanceService;
        private readonly PatientSearchService PatientSearchService;

        public ReferralService(CoreContext context)
        {
            Context = context;
            ReferralAcceptanceService = new ReferralAcceptanceService(context);
            PatientSearchService = new PatientSearchService(context);
        }


        public Referral GetReferral(int id)
        {
            var referral = Context.Referrals.SingleOrDefault(m => m.ID == id);
            return referral;
        }


        public Referral Insert(Referral referral)
        {
            Context.Referrals.Add(referral);
            var checklistItems = Context.ReferralChecklistItems.ToList();
            foreach (var cl in checklistItems)
            {
                referral.Checklist.Add(new ReferralChecklist { ChecklistItemID = cl.ID });
            }
            return Save(referral);
        }


        public Referral Save(Referral referral)
        {
            Context.SaveChanges();
            var r = ReferralAcceptanceService.HandleAcceptedReferral(referral);
            if (r)
            {
                PatientSearchService.UpdateEntry(referral.GeneratedPatientID.Value);
            }
            return referral;
        }


        public void Deactivate(int id)
        {
            var referral = Context.Referrals.SingleOrDefault(m => m.ID == id);
            if (referral == null)
            {
                throw new Exception($"Referral with ID '{id}' is not found");
            }
            referral.Active = false;
            Context.SaveChanges();
        }


        public void Delete(int id)
        {
            var referral = Context.Referrals.SingleOrDefault(m => m.ID == id);
            if (referral == null)
            {
                throw new Exception($"Referral with ID '{id}' is not found");
            }
            Context.Referrals.Remove(referral);
            Context.SaveChanges();
        }


        public ReferralNote CreateReferralNote()
        {
            return Context.ReferralNotes.Create();
        }


        public IEnumerable<ReferralDismissalReason> GetDismissalReasonTypes()
        {
            return Context.ReferralDismissalReasons.ToList();
        }


        public IEnumerable<ReferralEnumItem> GetEnums(string statusType)
        {
            return Context.ReferralEnums
                .Where(m => m.StatusType == statusType)
                .OrderBy(m => m.Order)
                .ToList();
        }


        public IEnumerable<Domain2.Infrastructure.Language> GetLanguages()
        {
            return Context.Languages.Where(m => m.Active).OrderBy(m => m.Description).ToList();
        }


        public IEnumerable<Domain2.Staff.Staff> GetOfficeStaffList()
        {
            return Context.Staff.Where(m => m.StaffActive).OrderBy(m => m.StaffLastName).ThenBy(m => m.StaffFirstName).ToList();
        }


        public IEnumerable<ReferralSourceType> GetSourceTypes()
        {
            return Context.ReferralSourceTypes.ToList();
        }


        public IEnumerable<Domain.General.State> GetStates()
        {
            return Domain.General.State.GetStates().OrderBy(s => s.Code).ToList();
        }


        public IEnumerable<Domain.Referrals.StatusDescriptor> GetStatusList()
        {
            return Domain.Referrals.StatusDescriptor.GetStatuses();
        }


        public IEnumerable<ReferralListItemModel> GetAllReferralItems(bool onlyFollowUps)
        {
            var e = Context.Referrals.Include("AssignedStaff").Include("Notes")
                .Where(m => m.Active && m.Followup == onlyFollowUps)
                .OrderByDescending(m => onlyFollowUps ? m.FollowupDate : m.DateCreated)
                .ToList();
            var o = e.Select(m =>
            {
                var obj = ToReferralListItemModel(m);
                obj.Status = (new List<int?>() {
                    m.InsuranceStatus,
                    m.IntakeStatus,
                    m.RxStatus,
                    m.InsuranceCardStatus,
                    m.EvaluationStatus,
                    m.PolicyBookStatus
                })
                .Where(n => n.HasValue)
                .Select(n =>
                {
                    return Context.ReferralEnums.Single(r => r.ID == n.Value);
                })
                .Select(t => ToStatusListItem(t));
                return obj;
            });
            return o;
        }


        public IEnumerable<ReferralListItemModel> GetDismissedReferralItems(bool onlyFollowUps)
        {
            var e = Context.Referrals.Include("AssignedStaff").Include("Notes")
                .Where(m => m.Active && (m.Status == ReferralStatus.Dismissed || m.Status == ReferralStatus.Accepted) && m.Followup == onlyFollowUps)
                .OrderByDescending(m => onlyFollowUps ? m.FollowupDate : m.DateCreated)
                .ToList();
            var o = e.Select(m =>
            {
                var obj = ToReferralListItemModel(m);
                obj.Status = (new List<int?>() {
                    m.InsuranceStatus,
                    m.IntakeStatus,
                    m.RxStatus,
                    m.InsuranceCardStatus,
                    m.EvaluationStatus,
                    m.PolicyBookStatus
                })
                .Where(n => n.HasValue)
                .Select(n =>
                {
                    return Context.ReferralEnums.Single(r => r.ID == n.Value);
                })
                .Select(t => ToStatusListItem(t));
                return obj;
            });
            return o;
        }


        public IEnumerable<Domain2.Insurances.Insurance> GetInsuranceCompanies()
        {
            return Context.Insurances.Where(x => x.Active).OrderBy(m => m.Name).ToList();
        }


        public IEnumerable<ReferralListItemModel> GetActiveReferralItems(bool onlyFollowUps)
        {
            var e = Context.Referrals.Include("AssignedStaff").Include("Notes")
                .Where(m => m.Active && m.Status != ReferralStatus.Accepted && m.Status != ReferralStatus.Dismissed && m.Followup == onlyFollowUps)
                .OrderByDescending(m => onlyFollowUps ? m.FollowupDate : m.DateCreated)
                .ToList();
            var o = e.Select(m =>
            {
                var obj = ToReferralListItemModel(m);
                obj.Status = (new List<int?>() {
                    m.InsuranceStatus,
                    m.IntakeStatus,
                    m.RxStatus,
                    m.InsuranceCardStatus,
                    m.EvaluationStatus,
                    m.PolicyBookStatus
                })
                .Where(n => n.HasValue)
                .Select(n =>
                {
                    return Context.ReferralEnums.Single(r => r.ID == n.Value);
                })
                .Select(t => ToStatusListItem(t));
                return obj;
            });
            return o;
        }


        private static ReferralListItemModel ToReferralListItemModel(Referral m)
        {
            var note = m.Notes.OrderByDescending(n => n.EntryDate).FirstOrDefault();
            var d = new ReferralListItemModel
            {
                ID = m.ID,
                AssignedStaff = m.AssignedStaff != null ? m.AssignedStaff.StaffLastName + ", " + m.AssignedStaff.StaffFirstName : string.Empty,
                LastName = m.LastName,
                FirstName = m.FirstName,
                InsuranceName = m.InsuranceCompanyName,
                BenefitCheck = m.BenefitCheck,
                StatusNote = note != null ? note.EntryDate.ToShortDateString() + " - " + note.Comments : string.Empty,
                City = m.City,
                State = m.State,
                DateCreated = m.DateCreated,
                FollowUpDate = m.FollowupDate
            };
            return d;
        }


        private static StatusListItem ToStatusListItem(ReferralEnumItem ei)
        {
            return new StatusListItem
            {
                Label = ei.Label,
                ColorCode = ei.ColorCode
            };
        }


    }
}