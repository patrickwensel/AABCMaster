using System.Collections.Generic;

namespace AABC.Web.Models.Cases
{
    public class CaseBillingCorrespondenceVM
    {
        public Dymeng.Framework.Web.Mvc.Views.IViewModelBase Base;

        public int Id { get; set; }
        public int BillingCorrespondenceCaseId { get; set; }
        public System.DateTime CorrespondenceDate { get; set; }
        public int CorrespondenceTypeId { get; set; }
        public string CorrespondenceTypeName { get; set; }
        public string ContactPerson { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Notes { get; set; }
        public string AttachmentFilename { get; set; }
        public IEnumerable<DevExpress.Web.UploadedFile> Attachments { get; set; }
        public IEnumerable<CaseBillingCorrespondenceTypeVM> CaseBillingCorrespondenceTypes { get; set; }
        public IEnumerable<Domain.OfficeStaff.OfficeStaff> StaffList { get; set; }
    }
}