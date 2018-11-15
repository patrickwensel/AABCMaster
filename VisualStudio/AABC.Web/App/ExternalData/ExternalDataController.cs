using AABC.Web.Helpers;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Dymeng.Framework.Web.Mvc.Controllers;
using Dymeng.Framework.Web.Mvc.Views;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    [AuthorizePermissions(Domain.Admin.Permissions.ProviderHoursView)]
    public class ExternalDataController : ContentBaseController
    {

        [HttpGet]
        public ActionResult CatalystHasData()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/ExternalData/CatalystHasData", "External Data - Catalyst Has Data");
            return GetView("CatalystHasData");
        }

        [HttpGet]
        public ActionResult CatalystTimesheet()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/ExternalData/CatalystTimesheet", "External Data - Catalyst Timesheet");
            return GetView("CatalystTimesheet");
        }


        public ActionResult TimesheetUpload() {
            string[] errors;

            UploadedFile[] files = UploadControlExtension.GetUploadedFiles("uploadCatalystTimesheet",
                Models.ExternalData.Catalyst.StudentAttendenceReportValidationSettings.Settings, out errors,
                CatalystTimesheetUpload_FileUploadComplete,
                CatalystTimesheetUpload_FilesUploadComplete);

            return null;
        }




        public void CatalystTimesheetUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {

            UploadedFile file = e.UploadedFile;

            // later we can run a validation on the file later, for now just assume it's
            // valid and let the actual import catch any issues
            file.IsValid = true;
            
        }

        public void CatalystTimesheetUpload_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e) {
            UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

            for (int i = 0; i < files.Length; i++) {
                if (files[i].IsValid && !string.IsNullOrWhiteSpace(files[i].FileName)) {

                    string path = System.Configuration.ConfigurationManager.AppSettings["CatalystImportDirectory"];
                    string fileName = System.IO.Path.GetRandomFileName() + ".xlsx";
                    string fullPath = Server.MapPath(System.IO.Path.Combine(path, fileName));

                    files[i].SaveAs(fullPath);

                    e.CallbackData = fileName;
                }
            }
        }

        [HttpPost]
        public ActionResult CatalystTimesheetProcess(string fileName) {

            string path = System.Configuration.ConfigurationManager.AppSettings["CatalystImportDirectory"];
            string fullPath = Server.MapPath(System.IO.Path.Combine(path, fileName));
            
            var importer = new DomainServices.Integrations.Catalyst.TimesheetImporter(fullPath);
            importer.Import();

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }











        public ActionResult CatalystStudentAttendanceUpload() {
            string[] errors;

            UploadedFile[] files = UploadControlExtension.GetUploadedFiles("uploadCatalystHoursSignoffData",
                Models.ExternalData.Catalyst.StudentAttendenceReportValidationSettings.Settings, out errors,
                CatalystStudentAttendanceUpload_FileUploadComplete,
                CatalystStudentAttendanceUpload_FilesUploadComplete);

            return null;
        }

        [HttpPost]
        public ActionResult CatalystStudentAttendanceProcess(string fileName) {

            string path = System.Configuration.ConfigurationManager.AppSettings["CatalystImportDirectory"];
            string fullPath = Server.MapPath(System.IO.Path.Combine(path, fileName));

            var importer = new DomainServices.Integrations.Catalyst.HasData(fullPath);
            importer.Import();

            var results = importer.Resolve();            
            var resultsModel = new Models.ExternalData.Catalyst.StudentAttendanceImportResults(results);

            return Content(JsonConvert.SerializeObject(resultsModel));            
        }


        [HttpPost]
        public ActionResult CatalystStudentAttendanceResults(string results) {            
            var model = JsonConvert.DeserializeObject<Models.ExternalData.Catalyst.StudentAttendanceImportResults>(results);            
            return PartialView("CatalystImportResultsPopup", model);            
        }


        public void CatalystStudentAttendanceUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {

            UploadedFile file = e.UploadedFile;
            file.IsValid = false;

            try {
                var hasData = new DomainServices.Integrations.Catalyst.HasData(file.FileNameInStorage);
                string errorText = null;

                if (!hasData.Validate(ref errorText)) { 
                    e.CallbackData = errorText;
                    throw new ArgumentException();
                } else {
                    file.IsValid = true;
                }

            }
            catch (Exception ex) {
                Dymeng.Framework.Exceptions.Handle(ex);
                file.IsValid = false;
                if (string.IsNullOrWhiteSpace(e.ErrorText)) {
                    e.CallbackData = ex.Message;
                }                
            }
            
        }

        public void CatalystStudentAttendanceUpload_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e) {
            UploadedFile[] files = ((MVCxUploadControl)sender).UploadedFiles;

            for (int i = 0; i < files.Length; i++) {
                if (files[i].IsValid && !string.IsNullOrWhiteSpace(files[i].FileName)) {
                    
                    string path = System.Configuration.ConfigurationManager.AppSettings["CatalystImportDirectory"];
                    string fileName = System.IO.Path.GetRandomFileName() + ".xlsx";
                    string fullPath = Server.MapPath(System.IO.Path.Combine(path, fileName));
                    
                    files[i].SaveAs(fullPath);

                    e.CallbackData = fileName;
                }
            }
        }

        
    }
}