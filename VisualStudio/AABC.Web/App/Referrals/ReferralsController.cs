using AABC.Domain2.Referrals;
using AABC.DomainServices.Utils;
using AABC.Web.App.Referrals.Models;
using Dymeng.Framework.Web.Mvc.Controllers;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.App.Referrals
{

    public partial class ReferralsController : ContentBaseController
    {

        private readonly ReferralService ReferralService;

        public ReferralsController()
        {
            ReferralService = new ReferralService(AppService.Current.DataContextV2);
        }


        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }


        public ActionResult Search()
        {
            return InnerSearch(false);
        }


        public ActionResult FollowUps()
        {
            return InnerSearch(true);
        }


        private ActionResult InnerSearch(bool onlyFollowUps)
        {
            var action = onlyFollowUps ? "FollowUps" : "Search";
            var filter = Request.Params["filter"] ?? "Active";
            IEnumerable<ReferralListItemModel> list = null;
            switch (filter)
            {
                default:
                    list = ReferralService.GetActiveReferralItems(onlyFollowUps);
                    break;
                case "Dismissed":
                    list = ReferralService.GetDismissedReferralItems(onlyFollowUps);
                    break;
                case "All":
                    list = ReferralService.GetAllReferralItems(onlyFollowUps);
                    break;
            }
            var model = new ReferralsListVM
            {
                Base = new ViewModelBase(PushState, $"/Referrals/{action}", "Referrals"),
                DetailList = list,
                IsShowingFollowUpsOnly = onlyFollowUps,
                Action = action
            };
            model.ListBase.CallbackFilterValue = filter;
            model.ListBase.GridTitlePanelSettings.Title = onlyFollowUps ? "Referral Follow Ups" : "Referrals";
            model.ListBase.GridTitlePanelSettings.ShowAddButton = true;
            model.ListBase.GridTitlePanelSettings.AddNewAction = "Create";
            model.ListBase.GridTitlePanelSettings.AddNewController = "Referrals";
            model.ListBase.GridTitlePanelSettings.FilterItems = (new string[] { "All", "Active", "Dismissed" })
                .Select(m => new GridTitlePanelFilterItem
                {
                    RouteUrl = $"/Referrals/{action}/?filter={m}",
                    Text = m,
                    IsActive = m == filter
                })
                .ToList();
            var isFilterCallback = Request.Params["isFilterCallback"] == "true";
            return isFilterCallback ? GetView("ListGrid", model) : GetView("List", model);
        }


        [HttpGet]
        public ActionResult Create()
        {
            var model = new ReferralEditVM
            {
                Base = new ViewModelBase(PushState, "/Referrals/Create", "New Referral Entry")
            };
            return GetView("Edit", model);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            var model = new ReferralEditVM
            {
                Base = new ViewModelBase(PushState, "/Referrals/Edit/" + id, "Referral Editing", "/Referrals/Search"),
                ReferralId = id
            };
            return GetView("Edit", model);
        }


        [HttpGet]
        public JsonResult GetReferralData(int referralId)
        {
            var referral = ReferralService.GetReferral(referralId);
            if (referral == null)
            {
                return Json(false);
            }
            var dto = Mapper.FromReferral(referral);
            return this.CamelCaseJson(dto, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetMetadata()
        {
            var model = new ReferralDataEditVM
            {
                SourceTypes = ReferralService.GetSourceTypes().Select(m => new ListItem<int>
                {
                    Value = m.ID,
                    Text = m.Name
                }),
                InsuranceStatuses = ReferralService.GetEnums(ReferralEnumItem.InsuranceStatus).Select(m => new ListItem<int> { Value = m.ID, Text = m.Label }),
                IntakeStatuses = ReferralService.GetEnums(ReferralEnumItem.IntakeStatus).Select(m => new ListItem<int> { Value = m.ID, Text = m.Label }),
                RxStatuses = ReferralService.GetEnums(ReferralEnumItem.RxStatus).Select(m => new ListItem<int> { Value = m.ID, Text = m.Label }),
                InsuranceCardStatuses = ReferralService.GetEnums(ReferralEnumItem.InsuranceCardStatus).Select(m => new ListItem<int> { Value = m.ID, Text = m.Label }),
                EvaluationStatuses = ReferralService.GetEnums(ReferralEnumItem.EvaluationStatus).Select(m => new ListItem<int> { Value = m.ID, Text = m.Label }),
                PolicyBookStatuses = ReferralService.GetEnums(ReferralEnumItem.PolicyBookStatus).Select(m => new ListItem<int> { Value = m.ID, Text = m.Label }),
                DismissalReasonTypes = ReferralService.GetDismissalReasonTypes().Select(m => new ListItem<int> { Value = m.ID, Text = m.Name }),
                States = ReferralService.GetStates().Select(m => new ListItem<string> { Value = m.Code, Text = m.Name }),
                Statuses = ReferralService.GetStatusList().Select(m => new ListItem<int> { Value = m.ID, Text = m.Name }),
                Languages = ReferralService.GetLanguages().Select(m => new ListItem<string> { Value = m.Code, Text = m.Description }),
                OfficeStaff = ReferralService.GetOfficeStaffList().Select(m => new ListItem<int> { Value = m.ID, Text = m.StaffLastName + ", " + m.StaffFirstName }),
                InsuranceCompanies = ReferralService.GetInsuranceCompanies().Select(m => new ListItem<int> { Value = m.ID, Text = m.Name }),
                InsuranceFundingTypes = EnumHelper.ToSelectListItem<Domain2.Cases.InsuranceFundingType>().Select(m => new ListItem<string> { Value = m.Text, Text = m.Text }),
                InsuranceBenefitTypes = EnumHelper.ToSelectListItem<Domain2.Cases.InsuranceBenefitType>().Select(m => new ListItem<string> { Value = m.Text, Text = m.Text }),
                GuardianRelationships = EnumHelper.ToSelectListItem<GuardianRelationship>()
            };
            return this.CamelCaseJson(model, JsonRequestBehavior.AllowGet);
        }


        /**********************
         *  POSTS
         * *******************/
        [HttpPost]
        public ActionResult Insert(ReferralDTO referral)
        {
            var referralDb = new Referral();
            Mapper.FromReferralDTO(referral, referralDb);
            ReferralService.Insert(referralDb);
            return this.CamelCaseJson(new { Success = true });
        }


        [HttpPost]
        public ActionResult Save(ReferralDTO referral)
        {
            var referralDb = ReferralService.GetReferral(referral.Id);
            if (referralDb == null)
            {
                return Json(new { success = false });
            }
            Mapper.FromReferralDTO(referral, referralDb);
            ReferralService.Save(referralDb);
            return this.CamelCaseJson(new { Success = true });
        }


        [HttpPost]
        public ActionResult Deactivate(int referralID)
        {
            ReferralService.Deactivate(referralID);
            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            ReferralService.Delete(id);
            return RedirectToAction("Search");
        }


        static class Mapper
        {

            public static void FromReferralDTO(ReferralDTO referral, Referral referralDb)
            {
                referralDb.Status = MapStatus(referral.Status);
                referralDb.DismissalReasonID = referral.DismissalReasonId;
                referralDb.DismissalNote = referral.DismissalReason;
                referralDb.DismissalNoteExtended = referral.DismissalReasonNotes;
                referralDb.Followup = referral.FollowUp;
                referralDb.FollowupDate = referral.FollowUpDate;
                referralDb.SourceTypeID = referral.SourceTypeId;
                referralDb.SourceName = referral.SourceName;
                referralDb.ReferrerNotes = referral.ReferrerNotes;
                referralDb.AssignedStaffID = referral.AssignedStaffId;
                referralDb.LastName = referral.LastName;
                referralDb.FirstName = referral.FirstName;
                referralDb.DateOfBirth = referral.DateOfBirth;
                referralDb.PrimaryLanguage = referral.PrimaryLanguage;
                referralDb.AreaOfConcern = referral.AreaOfConcern;
                referralDb.ServicesRequested = referral.ServicesRequested;
                referralDb.StatusNotes = referral.StatusNotes;

                referralDb.GuardianLastName = referral.GuardianLastName;
                referralDb.GuardianFirstName = referral.GuardianFirstName;
                referralDb.GuardianRelationshipID = referral.GuardianRelationshipId;
                referralDb.GuardianEmail = referral.GuardianEmail;
                referralDb.GuardianCellPhone = referral.GuardianCellPhone;
                referralDb.GuardianHomePhone = referral.GuardianHomePhone;
                referralDb.GuardianWorkPhone = referral.GuardianWorkPhone;
                referralDb.GuardianNotes = referral.GuardianNotes;
                referralDb.Guardian2FirstName = referral.Guardian2FirstName;
                referralDb.Guardian2LastName = referral.Guardian2LastName;
                referralDb.Guardian2Email = referral.Guardian2Email;
                referralDb.Guardian2CellPhone = referral.Guardian2CellPhone;
                referralDb.Guardian2HomePhone = referral.Guardian2HomePhone;
                referralDb.Guardian2WorkPhone = referral.Guardian2WorkPhone;
                referralDb.Guardian2RelationshipID = referral.Guardian2RelationshipId;
                referralDb.Guardian2Notes = referral.Guardian2Notes;
                referralDb.Guardian3FirstName = referral.Guardian3FirstName;
                referralDb.Guardian3LastName = referral.Guardian3LastName;
                referralDb.Guardian3RelationshipID = referral.Guardian3RelationshipId;
                referralDb.Guardian3CellPhone = referral.Guardian3CellPhone;
                referralDb.Guardian3HomePhone = referral.Guardian3HomePhone;
                referralDb.Guardian3WorkPhone = referral.Guardian3WorkPhone;
                referralDb.Guardian3Email = referral.Guardian3Email;
                referralDb.Guardian3Notes = referral.Guardian3Notes;
                referralDb.PhysicianName = referral.PhysicianName;
                referralDb.PhysicianAddress = referral.PhysicianAddress;
                referralDb.PhysicianEmail = referral.PhysicianEmail;
                referralDb.PhysicianContact = referral.PhysicianContact;
                referralDb.PhysicianFax = referral.PhysicianFax;
                referralDb.PhysicianNotes = referral.PhysicianNotes;
                referralDb.PhysicianPhone = referral.PhysicianPhone;

                referralDb.Email = referral.Email;
                referralDb.Phone = referral.Phone;
                referralDb.Address1 = referral.Address1;
                referralDb.Address2 = referral.Address2;
                referralDb.City = referral.City;
                referralDb.State = referral.State;
                referralDb.ZipCode = referral.ZipCode;
                referralDb.InsuranceCompanyName = referral.CompanyName;
                referralDb.InsuranceMemberID = referral.MemberId;
                referralDb.InsurancePrimaryCardholderDOB = referral.PrimaryCardholderDateOfBirth;
                referralDb.InsurancePrimaryCardholderName = referral.PrimaryCardholderName;
                referralDb.InsuranceProviderPhone = referral.CompanyProviderPhone;
                referralDb.BenefitCheck = referral.BenefitCheck;
                referralDb.InsuranceStatus = referral.InsuranceStatus;
                referralDb.IntakeStatus = referral.IntakeStatus;
                referralDb.RxStatus = referral.RxStatus;
                referralDb.InsuranceCardStatus = referral.InsuranceCardStatus;
                referralDb.EvaluationStatus = referral.EvaluationStatus;
                referralDb.PolicyBookStatus = referral.PolicyBookStatus;
                referralDb.InsuranceCompanyID = referral.InsuranceCompanyId;
                referralDb.FundingType = referral.FundingType;
                referralDb.BenefitType = referral.BenefitType;
                referralDb.CoPayAmount = referral.CoPayAmount;
                referralDb.CoInsuranceAmount = referral.CoInsuranceAmount;
                referralDb.DeductibleTotal = referral.DeductibleTotal;

                if (referral.Checklist != null)
                {
                    foreach (var cl in referral.Checklist)
                    {
                        var clDb = referralDb.Checklist.SingleOrDefault(m => m.ID == cl.Id);
                        if (clDb != null)
                        {
                            clDb.IsComplete = cl.IsComplete;
                        }
                    }
                }
            }


            public static ReferralDTO FromReferral(Referral referral)
            {
                var dto = new ReferralDTO
                {
                    Id = referral.ID,
                    DateCreated = referral.DateCreated,
                    Status = MapStatus(referral.Status),
                    StatusNotes = referral.StatusNotes,
                    DismissalReasonId = referral.DismissalReasonID,
                    DismissalReason = referral.DismissalNote,
                    DismissalReasonNotes = referral.DismissalNoteExtended,
                    FollowUp = referral.Followup,
                    FollowUpDate = referral.FollowupDate,
                    SourceTypeId = referral.SourceTypeID,
                    SourceName = referral.SourceName,
                    ReferrerNotes = referral.ReferrerNotes,
                    AssignedStaffId = referral.AssignedStaffID,
                    LastName = referral.LastName,
                    FirstName = referral.FirstName,
                    DateOfBirth = referral.DateOfBirth,
                    PrimaryLanguage = referral.PrimaryLanguage,
                    AreaOfConcern = referral.AreaOfConcern,
                    ServicesRequested = referral.ServicesRequested,
                    Email = referral.Email,
                    Phone = referral.Phone,
                    Address1 = referral.Address1,
                    Address2 = referral.Address2,
                    City = referral.City,
                    State = referral.State,
                    ZipCode = referral.ZipCode,
                    CompanyName = referral.InsuranceCompanyName,
                    MemberId = referral.InsuranceMemberID,
                    PrimaryCardholderDateOfBirth = referral.InsurancePrimaryCardholderDOB,
                    PrimaryCardholderName = referral.InsurancePrimaryCardholderName,
                    CompanyProviderPhone = referral.InsuranceProviderPhone,
                    BenefitCheck = referral.BenefitCheck,
                    InsuranceStatus = referral.InsuranceStatus,
                    IntakeStatus = referral.IntakeStatus,
                    RxStatus = referral.RxStatus,
                    InsuranceCardStatus = referral.InsuranceCardStatus,
                    EvaluationStatus = referral.EvaluationStatus,
                    PolicyBookStatus = referral.PolicyBookStatus,
                    InsuranceCompanyId = referral.InsuranceCompanyID,
                    FundingType = referral.FundingType,
                    BenefitType = referral.BenefitType,
                    CoPayAmount = referral.CoPayAmount,
                    CoInsuranceAmount = referral.CoInsuranceAmount,
                    DeductibleTotal = referral.DeductibleTotal,

                    GuardianLastName = referral.GuardianLastName,
                    GuardianFirstName = referral.GuardianFirstName,
                    GuardianRelationshipId = referral.GuardianRelationshipID,
                    GuardianEmail = referral.GuardianEmail,
                    GuardianCellPhone = referral.GuardianCellPhone,
                    GuardianHomePhone = referral.GuardianHomePhone,
                    GuardianWorkPhone = referral.GuardianWorkPhone,
                    GuardianNotes = referral.GuardianNotes,
                    Guardian2FirstName = referral.Guardian2FirstName,
                    Guardian2LastName = referral.Guardian2LastName,
                    Guardian2Email = referral.Guardian2Email,
                    Guardian2CellPhone = referral.Guardian2CellPhone,
                    Guardian2HomePhone = referral.Guardian2HomePhone,
                    Guardian2WorkPhone = referral.Guardian2WorkPhone,
                    Guardian2RelationshipId = referral.Guardian2RelationshipID,
                    Guardian2Notes = referral.Guardian2Notes,
                    Guardian3FirstName = referral.Guardian3FirstName,
                    Guardian3LastName = referral.Guardian3LastName,
                    Guardian3RelationshipId = referral.Guardian3RelationshipID,
                    Guardian3CellPhone = referral.Guardian3CellPhone,
                    Guardian3HomePhone = referral.Guardian3HomePhone,
                    Guardian3WorkPhone = referral.Guardian3WorkPhone,
                    Guardian3Email = referral.Guardian3Email,
                    Guardian3Notes = referral.Guardian3Notes,
                    PhysicianName = referral.PhysicianName,
                    PhysicianAddress = referral.PhysicianAddress,
                    PhysicianEmail = referral.PhysicianEmail,
                    PhysicianContact = referral.PhysicianContact,
                    PhysicianFax = referral.PhysicianFax,
                    PhysicianNotes = referral.PhysicianNotes,
                    PhysicianPhone = referral.PhysicianPhone
                };

                if (!referral.GuardianRelationshipID.HasValue && !string.IsNullOrEmpty(referral.GuardianRelationship))
                {
                    if (Enum.TryParse(referral.GuardianRelationship, true, out GuardianRelationship relationship))
                    {
                        dto.GuardianRelationshipId = (int)relationship;
                    }
                }
                dto.Checklist = referral.Checklist.Select(m => FromChecklist(m));
                return dto;
            }


            private static ReferralStatus MapStatus(Domain.Referrals.ReferralStatus s)
            {
                switch (s)
                {
                    case Domain.Referrals.ReferralStatus.Accepted:
                        return ReferralStatus.Accepted;
                    case Domain.Referrals.ReferralStatus.Dismissed:
                        return ReferralStatus.Dismissed;
                    case Domain.Referrals.ReferralStatus.InProcess:
                        return ReferralStatus.InProcess;
                    case Domain.Referrals.ReferralStatus.New:
                        return ReferralStatus.New;
                    case Domain.Referrals.ReferralStatus.SpecialAttention:
                        return ReferralStatus.SpecialAttention;
                    default:
                        throw new Exception("Could not map status");
                }
            }


            private static Domain.Referrals.ReferralStatus MapStatus(ReferralStatus s)
            {
                switch (s)
                {
                    case ReferralStatus.Accepted:
                        return Domain.Referrals.ReferralStatus.Accepted;
                    case ReferralStatus.Dismissed:
                        return Domain.Referrals.ReferralStatus.Dismissed;
                    case ReferralStatus.InProcess:
                        return Domain.Referrals.ReferralStatus.InProcess;
                    case ReferralStatus.New:
                        return Domain.Referrals.ReferralStatus.New;
                    case ReferralStatus.SpecialAttention:
                        return Domain.Referrals.ReferralStatus.SpecialAttention;
                    default:
                        throw new Exception("Could not map status");
                }
            }


            private static ReferralCheckListDTO FromChecklist(ReferralChecklist cl)
            {
                return new ReferralCheckListDTO
                {
                    Id = cl.ID,
                    ChecklistItemDescription = cl.ChecklistItem.Description,
                    IsComplete = cl.IsComplete
                };
            }


        }


    }
}

