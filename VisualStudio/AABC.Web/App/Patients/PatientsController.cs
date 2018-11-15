using AABC.DomainServices.Patients;
using AABC.Web.App.Patients.Models;
using AABC.Web.Models.Patients;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    public class PatientsController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        /**********************
         *  POSTS
         * *******************/
        [HttpPost]
        public ActionResult SetRestaffReasonValue(int caseID, int reasonID)
        {
            repository.SetRestaffReasonID(caseID, reasonID);
            return Content("ok");
        }


        [HttpPost]
        public ActionResult GridQuickEditDateUpdate(int caseID, string field, DateTime? newDate)
        {
            switch (field)
            {
                case "StartDate":
                    repository.UpdateStartDate(caseID, newDate);
                    break;
                default:
                    throw new ArgumentException();
            }
            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult GridQuickEditCheckboxUpdate(int caseID, string field, bool newValue)
        {
            switch (field)
            {
                case "intake":
                    repository.UpdateHasIntake(caseID, newValue);
                    break;
                case "rx":
                    repository.UpdateHasPrescription(caseID, newValue);
                    break;
                case "needsRestaffing":
                    repository.UpdateNeedsRestaffing(caseID, newValue);
                    break;
                default:
                    throw new ArgumentException();
            }
            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult Edit(PatientVM model)
        {
            model.Base = new ViewModelBase(PushState, "/Patients/Edit/" + model.ID, "Patient/Case Entry", "/Patients/Search");
            if (!model.ID.HasValue)
            {
                model.ViewHelper.IsNewPatientEntry = true;
            }
            model.ViewHelper.GuardianRelationships = repository.GetGuardianRelationships();
            model.ViewHelper.Insurances = repository.GetInsuranceList();
            return SaveFullAction(model, model.ViewHelper, "Search", "Edit", "ErrorPartial", () => repository.SavePatient(model));
        }


        public static readonly DevExpress.Web.UploadControlValidationSettings ValidationSettings = new DevExpress.Web.UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".pdf", ".doc", ".docx", ".txt", ".rtf" },
            MaxFileSize = 20971520,
        };


        /**********************
         *  GETS
         * *******************/
        [HttpGet]
        public ActionResult SetRestaffReason(int id)
        {
            var model = new RestaffReasonVM
            {
                ID = id,
                SelectionList = repository.GetRestaffReasonItems(),
                ReasonID = repository.GetRestaffReason(id)
            };
            return PartialView("RestaffReasonPopup", model);
        }


        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }


        [HttpGet]
        public ActionResult Search(bool isFilterCallback = false)
        {
            PatientsListVM<PatientsListItem2VM> model;
            string pageTitle = "";
            string gridHeaderTitle = "";
            string viewName = "List";
            string activeFilterText = "";
            string filter = Request.Params["filter"] ?? "Active";
            switch (filter)
            {
                case "Discharged":
                    model = new PatientsListVM<PatientsListItem2VM>(ListViewType.Discharged)
                    {
                        DetailList = repository.GetDischargedPatientListItems()
                    };
                    pageTitle = "Patients - Discharged";
                    gridHeaderTitle = "Patients (Discharged)";
                    activeFilterText = "Discharged";
                    model.ListBase.CallbackFilterValue = "Discharged";
                    break;
                default: // "Active"
                    model = new PatientsListVM<PatientsListItem2VM>(ListViewType.Active)
                    {
                        DetailList = repository.GetPatientListItems()
                    };
                    pageTitle = "Patients - Active";
                    gridHeaderTitle = "Patients (Active)";
                    activeFilterText = "Active";
                    model.ListBase.CallbackFilterValue = "Active";
                    break;
            }
            model.Base = new ViewModelBase(PushState, "/Patients/Search", pageTitle);
            model.ListBase.GridTitlePanelSettings.AddNewAction = "Create";
            model.ListBase.GridTitlePanelSettings.AddNewController = "Patients";
            model.ListBase.GridTitlePanelSettings.Title = gridHeaderTitle;
            model.ListBase.GridTitlePanelSettings.ShowAddButton = true;
            model.ListBase.GridTitlePanelSettings.FilterItems = new List<GridTitlePanelFilterItem>()
            {
                new GridTitlePanelFilterItem() { RouteUrl = "/Patients/Search/?filter=Active", Text = "Active" },
                new GridTitlePanelFilterItem() { RouteUrl = "/Patients/Search/?filter=Discharged", Text = "Discharged" }
            };
            var activeFilter = model.ListBase.GridTitlePanelSettings.FilterItems.Where(x => x.Text == activeFilterText).FirstOrDefault();
            if (activeFilter != null)
            {
                activeFilter.IsActive = true;
            }

            if (isFilterCallback)
            {
                return GetView(viewName + "Grid", model);
            }
            else
            {
                return GetView(viewName, model);
            }
        }


        [HttpGet]
        public ActionResult Create()
        {
            var model = new PatientVM
            {
                Base = new ViewModelBase(PushState, "/Patients/Create", "Patient/Case Entry")
            };
            model.ViewHelper.IsNewPatientEntry = true;
            model.ViewHelper.GuardianRelationships = repository.GetGuardianRelationships();
            model.ViewHelper.Insurances = repository.GetInsuranceList();
            return GetView("Edit", model);
        }


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return Create();
            }
            // the ID passed here is the CaseID
            // get the Patient ID instead
            int? patientID = repository.GetPatientByCase(id.Value);
            if (!patientID.HasValue)
            {
                return Create();
            }
            var model = repository.GetPatient(patientID.Value);
            model.CaseId = id;
            model.ViewHelper.GuardianRelationships = repository.GetGuardianRelationships();
            model.ViewHelper.Insurances = repository.GetInsuranceList();
            model.Base = new ViewModelBase(PushState, "/Patients/Edit/" + id.Value, "Patient/Case Edit", "/Patients/Search");
            return GetView("Edit", model);
        }


        public ActionResult CaseHighRiskLabel(int CaseId)
        {
            int? patientID = repository.GetPatientByCase(CaseId);
            if (!patientID.HasValue)
            {
                return new EmptyResult();
            }
            var model = repository.GetPatient(patientID.Value);
            return PartialView(model);
        }


        [HttpGet]
        public ActionResult GetPrescription(int id)
        {
            var model = repository.GetPatient(id);
            string filePath = AppService.Current.Settings.UploadDirectory + model.PrescriptionLocation;
            Response.AddHeader("Content-Disposition", "inline; filename=" + model.PrescriptionFileName);
            return File(System.IO.File.ReadAllBytes(filePath), GetMimeType(model.PrescriptionFileName));
        }


        public ActionResult PatientSelect()
        {
            return PartialView("PatientSelect");
        }


        public ActionResult PatientSelectGrid()
        {
            var model = svcPatient.GetAll();
            return PartialView("PatientSelectGrid", model);
        }


        private string GetMimeType(string fileName)
        {
            string extension = fileName.Substring(fileName.LastIndexOf('.'));
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                default:
                    return "application/octet";
            }
        }


        /**********************
         *  DELETES
         * *******************/
        [HttpPost]
        public ActionResult Delete(int id)
        {
            repository.DeletePatient(id);
            return RedirectToAction("Search");
        }


        /**********************
         *  DEVEX/CALLBACKS
         * *******************/
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult SearchGridFilter()
        {
            return Search(true);
        }


        /**********************
         *  HELPERS
         * *******************/




        /**********************
         *  CONTROLLER SETUP
         * *******************/
        private readonly PatientService svcPatient;
        private readonly Repositories.IPatientRepository repository;

        public PatientsController()
        {
            repository = new Repositories.PatientRepository();
            svcPatient = new PatientService(AppService.Current.DataContextV2);
        }

        public PatientsController(Repositories.IPatientRepository patientRepository)
        {
            repository = patientRepository;
            svcPatient = new PatientService(AppService.Current.DataContextV2);
        }

    }
}