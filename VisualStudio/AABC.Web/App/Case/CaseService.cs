using AABC.Data.Models;
using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.DomainServices.Cases;
using AABC.DomainServices.HoursResolution;
using AABC.DomainServices.HoursResolution.Repositories;
using AABC.DomainServices.Staffing;
using AABC.Web.App.Case.Models;
using AABC.Web.App.Hours.Models;
using AABC.Web.App.Patients;
using AABC.Web.Helpers;
using AABC.Web.Models.Cases;
using AABC.Web.Reports;
using DevExpress.Web.Mvc;
using DevExpress.XtraReports.UI;
using Dymeng.Framework;
using Dymeng.Framework.Validation;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace AABC.Web.App.Case
{
    public class CaseService
    {

        public enum PatientHoursReportConfigurations
        {
            Default,
            SignLine,
            SupervisingBCBA
        }

        private readonly CoreContext _context;
        private readonly CoreEntityModel _contextOld;
        private readonly PatientSearchService PatientSearchService;
        private readonly IStaffingService StaffingService;

        public CaseService()
        {
            _context = AppService.Current.DataContextV2;
            _contextOld = AppService.Current.DataContext;
            PatientSearchService = new PatientSearchService(_context);
            StaffingService = new StaffingService(_context);
        }


        public MemoryStream GetPatientHoursReportsForPeriod(int caseID, DateTime period, PatientHoursReportConfigurations configuration = PatientHoursReportConfigurations.Default)
        {
            var startDate = period;
            var endDate = period.AddMonths(1).AddDays(-1);
            var tempDir = ConfigurationManager.AppSettings["TempDirectory"];
            string reportBaseName = "PatientHoursReport_";
            switch (configuration)
            {
                case PatientHoursReportConfigurations.SignLine:
                    reportBaseName += "Signline_";
                    break;
                case PatientHoursReportConfigurations.SupervisingBCBA:
                    reportBaseName += "SupervisingBCBA_";
                    break;
            }
            var providerIDs = _context.Hours.Where(x => x.CaseID == caseID && x.Date >= startDate && x.Date <= endDate)
                .Select(x => x.ProviderID)
                .Distinct()
                .ToArray();
            var files = new List<string>();
            foreach (int providerID in providerIDs)
            {
                XtraReport report;
                switch (configuration)
                {
                    case PatientHoursReportConfigurations.SignLine:
                        report = ReportService.GetPatientHoursReportWithSignLine(caseID, startDate, endDate, providerID);
                        break;
                    case PatientHoursReportConfigurations.SupervisingBCBA:
                        report = ReportService.GetPatientHoursReportWithSupervisingBCBA(caseID, startDate, endDate, providerID);
                        break;
                    default:
                        report = ReportService.GetPatientHoursReport(caseID, startDate, endDate, providerID);
                        break;
                }
                using (var stream = new MemoryStream())
                {
                    var filename = Path.Combine(tempDir, reportBaseName + caseID + "_" + providerID + "_" + startDate.ToString("yyyyMMddHHmmss") + ".pdf");
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    report.ExportToPdf(filename);
                    files.Add(filename);
                }
            }

            // merge the files
            var targetPath = Path.Combine(tempDir, reportBaseName + caseID + "_" + startDate.ToString("yyyyMMddHHmmss") + ".pdf");
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            MergePDFs(targetPath, files.ToArray());
            using (var stream = new MemoryStream())
            {
                var docBytes = File.ReadAllBytes(targetPath);
                stream.Write(docBytes, 0, docBytes.Length);
                return stream;
            }
        }


        public MemoryStream GetParentHoursReportsForPeriod(int caseID, DateTime period)
        {
            var startDate = period;
            var endDate = period.AddMonths(1).AddDays(-1);
            var tempDir = ConfigurationManager.AppSettings["TempDirectory"];
            var providerIDs = _context.Hours.Where(x => x.CaseID == caseID && x.Date >= startDate && x.Date <= endDate)
                .Select(x => x.ProviderID)
                .Distinct()
                .ToArray();
            var files = new List<string>();
            foreach (int providerID in providerIDs)
            {
                var report = ReportService.GetParentHoursReport(caseID, startDate, endDate, providerID);
                using (var stream = new MemoryStream())
                {
                    string filename = Path.Combine(tempDir, "ParentHoursReport_" + caseID + "_" + providerID + "_" + startDate.ToString("yyyyMMddHHmmss") + ".pdf");
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    report.ExportToPdf(filename);
                    files.Add(filename);
                }
            }

            // merge the files
            var targetPath = Path.Combine(tempDir, "ParentHoursReport_" + caseID + "_" + startDate.ToString("yyyyMMddHHmmss") + ".pdf");
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            MergePDFs(targetPath, files.ToArray());
            using (var stream = new MemoryStream())
            {
                var docBytes = File.ReadAllBytes(targetPath);
                stream.Write(docBytes, 0, docBytes.Length);
                return stream;
            }
        }


        public CaseMonthlyPeriod GetCaseMonthlyPeriod(int caseID, int month, int year)
        {
            var caseMonthlyPeriod = _contextOld.CaseMonthlyPeriods.Where(x =>
                x.CaseID == caseID
                && x.PeriodFirstDayOfMonth.Month == month
                && x.PeriodFirstDayOfMonth.Year == year).SingleOrDefault();
            return caseMonthlyPeriod;
        }


        public void SaveCaseMonthlyPeriod(WatchDetailPopupVM model)
        {
            var caseMonthlyPeriod = _contextOld.CaseMonthlyPeriods.Where(x => x.CaseID == model.CaseID && x.PeriodFirstDayOfMonth.Month == model.Month && x.PeriodFirstDayOfMonth.Year == model.Year).SingleOrDefault();
            if (caseMonthlyPeriod != null)
            {
                caseMonthlyPeriod.WatchComment = model.Comments;
                caseMonthlyPeriod.WatchIgnore = model.Ignore;
            }
            else
            {
                _contextOld.CaseMonthlyPeriods.Add(new CaseMonthlyPeriod
                {
                    CaseID = model.CaseID,
                    DateCreated = DateTime.Now,
                    PeriodFirstDayOfMonth = new DateTime(model.Year, model.Month, 1),
                    WatchComment = model.Comments,
                    WatchIgnore = model.Ignore
                    //rv = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 }
                });
            }
            _contextOld.SaveChanges();
        }


        public IEnumerable<CaseProviderListItem> GetCaseProviderListItems(int caseID, bool showAll)
        {
            var items = _context.CaseProviders.Where(m => m.CaseID == caseID);
            if (!showAll)
            {
                items = items.Where(m => m.Active);
            }
            return items.ToList().Select(p => new CaseProviderListItem
            {
                ProviderID = p.Provider.ID,
                Name = p.Provider.FirstName + " " + p.Provider.LastName,
                TypeID = p.Provider.ProviderTypeID,
                Type = p.Provider.GetProviderTypeFullCode(),
                Phone = p.Provider.Phone,
                Email = p.Provider.Email,
                ID = p.ID,
                Active = p.Active,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsAssessor = p.IsAssessor,
                IsAuthorizedBCBA = p.IsAuthorizedBCBA,
                IsSupervisor = p.IsSupervisor
            });
        }


        public IEnumerable<CaseAuthorizationDTO> GetAllAuthsByCase(int caseID)
        {
            var data = _context.Database.SqlQuery<CaseAuthorizationDTO>("EXEC dbo.GetAllAuthsByCase @CaseID", new SqlParameter("@CaseID", caseID)).ToList();
            return data;
        }


        public IEnumerable<ProviderDropdownListItem> GetProvidersDropdown()
        {
            var items = _context.Providers
                .OrderBy(m => m.ProviderType.Code).ThenBy(m => m.FirstName)
                .ToList()
                .Select(m => new ProviderDropdownListItem
                {
                    ID = m.ID,
                    Type = m.GetProviderTypeFullCode(),
                    Name = m.FirstName + " " + m.LastName
                });
            return items;
        }


        public string GetPatientName(int caseID)
        {
            var p = _contextOld.Cases.Find(caseID)?.Patient;
            var n = p.PatientFirstName + " " + p.PatientLastName;
            return n;
        }


        public void CaseProviderToggleIsAuthorizedBCBA(int caseProviderID)
        {
            var d = _context.CaseProviders.Find(caseProviderID);
            d.IsAuthorizedBCBA = !d.IsAuthorizedBCBA;
            if (!d.VerifyRoles(DateTime.Now))
            {
                _context.Entry(d).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidOperationException("Verification of role failed");
            }
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(d.Case.PatientID);
        }


        public void CaseProviderToggleIsAssessor(int caseProviderID)
        {
            var d = _context.CaseProviders.Find(caseProviderID);
            d.IsAssessor = !d.IsAssessor;
            if (!d.VerifyRoles(DateTime.Now))
            {
                _context.Entry(d).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidOperationException("Verification of role failed");
            }
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(d.Case.PatientID);
        }


        public void CaseProviderToggleIsSupervisor(int caseProviderID)
        {
            var d = _context.CaseProviders.Find(caseProviderID);
            d.IsSupervisor = !d.IsSupervisor;
            if (!d.VerifyRoles(DateTime.Now))
            {
                _context.Entry(d).State = System.Data.Entity.EntityState.Detached;
                throw new InvalidOperationException("Verification of role failed");
            }
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(d.Case.PatientID);
        }


        public void CaseProviderToggleActive(int caseProviderID)
        {
            var d = _contextOld.CaseProviders.Find(caseProviderID);
            d.Active = !d.Active;
            _contextOld.SaveChanges();
            PatientSearchService.UpdateEntry(d.Case.PatientID);
            StaffingService.PerformCheckByCaseId(d.CaseID);
        }


        public void CaseProviderSetEndDate(int caseProviderID, DateTime? endDate)
        {
            var d = _context.CaseProviders.Find(caseProviderID);
            d.EndDate = endDate;
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(d.Case.PatientID);
            StaffingService.PerformCheckByCaseId(d.CaseID);
        }


        public void CaseProviderSetStartDate(int caseProviderID, DateTime? startDate)
        {
            var d = _context.CaseProviders.Find(caseProviderID);
            d.StartDate = startDate;
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(d.Case.PatientID);
            StaffingService.PerformCheckByCaseId(d.CaseID);
        }


        #region CaseInsurance
        public IEnumerable<CaseInsuranceHistoryDTO> GetInsuranceHistory(int caseId)
        {
            var insurance = _context.CaseInsurances
                .Where(c => c.CaseID == caseId)
                .OrderByDescending(c => c.DatePlanTerminated)
                .ToList()
                .Select(m => Mapper.ToInsuranceHistoryDTO(m));
            return insurance;
        }


        public CaseInsuranceDTO GetInsurance(int id)
        {
            CaseInsuranceDTO dto = null;
            var insurance = _context.CaseInsurances.Where(c => c.ID == id).SingleOrDefault();
            if (insurance != null)
            {
                dto = Mapper.ToInsuranceDTO(insurance);
            }
            return dto;
        }


        public CaseInsuranceDTO InsertInsurance(CaseInsuranceDTO dto)
        {
            var caseInsurance = _context.CaseInsurances.Create();
            caseInsurance.CaseID = dto.InsuranceCaseId;
            Mapper.ToCaseInsurance(dto, caseInsurance);
            if (!new CaseInsuranceValidationService(_context).ValidateInsuranceUpdate(caseInsurance.CaseID, caseInsurance))
            {
                throw new InvalidOperationException("The insurance cannot be applied because it will overlap with an existing insurance entry.");
            }
            _context.CaseInsurances.Add(caseInsurance);
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(caseInsurance.Case.PatientID);
            return Mapper.ToInsuranceDTO(caseInsurance);
        }


        public CaseInsuranceDTO SaveInsurance(CaseInsuranceDTO dto)
        {
            var caseInsurance = _context.CaseInsurances.SingleOrDefault(m => m.ID == dto.Id);
            if (caseInsurance == null)
            {
                throw new Exception();
            }
            Mapper.ToCaseInsurance(dto, caseInsurance);
            if (!new CaseInsuranceValidationService(_context).ValidateInsuranceUpdate(caseInsurance.CaseID, caseInsurance))
            {
                throw new InvalidOperationException("The insurance cannot be applied because it will overlap with an existing insurance entry.");
            }
            _context.SaveChanges();
            PatientSearchService.UpdateEntry(caseInsurance.Case.PatientID);
            return Mapper.ToInsuranceDTO(caseInsurance);
        }

        public IEnumerable<Domain2.Insurances.Insurance> GetInsurances()
        {
            return _context.Insurances
                .Where(x => x.Active)
                .OrderBy(x => x.Name)
                .ToList();
        }


        public IEnumerable<Domain2.Insurances.LocalCarrier> GetLocalCarriers(int insuranceId)
        {
            return _context.InsuranceLocalCarriers
                .Where(m => m.InsuranceID == insuranceId)
                .OrderBy(x => x.Name)
                .ToList();
        }
        #endregion


        #region CaseInsuranceMaxOutOfPocket
        public CaseInsuranceMaxOutOfPocketDTO InsertCaseInsuranceMaxOutOfPocket(CaseInsuranceMaxOutOfPocketDTO dto)
        {
            var caseInsuranceMaxOutOfPocket = _context.CaseInsurancesMaxOutOfPocket.Create();
            caseInsuranceMaxOutOfPocket.CaseInsuranceId = dto.CaseInsuranceId;
            Mapper.ToCaseInsuranceMaxOutOfPocket(dto, caseInsuranceMaxOutOfPocket);
            _context.CaseInsurancesMaxOutOfPocket.Add(caseInsuranceMaxOutOfPocket);
            _context.SaveChanges();
            return Mapper.ToCaseInsuranceMaxOutOfPocketDTO(caseInsuranceMaxOutOfPocket);
        }


        public CaseInsuranceMaxOutOfPocketDTO SaveCaseInsuranceMaxOutOfPocket(CaseInsuranceMaxOutOfPocketDTO dto)
        {
            var caseInsuranceMaxOutOfPocket = _context.CaseInsurancesMaxOutOfPocket.SingleOrDefault(m => m.Id == dto.Id);
            if (caseInsuranceMaxOutOfPocket == null)
            {
                throw new Exception();
            }
            Mapper.ToCaseInsuranceMaxOutOfPocket(dto, caseInsuranceMaxOutOfPocket);
            _context.SaveChanges();
            return Mapper.ToCaseInsuranceMaxOutOfPocketDTO(caseInsuranceMaxOutOfPocket);
        }


        public bool DeleteCaseInsuranceMaxOutOfPocket(int id)
        {
            var m = _context.CaseInsurancesMaxOutOfPocket.Where(p => p.Id == id).SingleOrDefault();
            if (m != null)
            {
                _context.CaseInsurancesMaxOutOfPocket.Remove(m);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion


        #region CaseInsurancePayment

        public IEnumerable<CaseInsurancePaymentVM> GetCaseInsurancePayments(int CaseInsuranceId)
        {
            var items = _context.CaseInsurancePayments
                .Where(c => c.CaseInsurance.CaseID == CaseInsuranceId)
                .OrderByDescending(c => c.PaymentDate)
                .ToList()
                .Select(Transform);
            return items;
        }


        public CaseInsurancePaymentVM GetCaseInsurancePayment(int CaseInsurancePaymentId)
        {
            var n = _context.CaseInsurancePayments.Where(c => c.Id == CaseInsurancePaymentId).FirstOrDefault();
            if (n != null)
            {
                return Transform(n);
            }
            else
            {
                n = _context.CaseInsurancePayments.Create();
                return Transform(n);
            }
        }


        public void SaveCaseInsurancePaymentVM(CaseInsurancePaymentVM n)
        {
            var m = Transform(n);
            if (m.Id == 0)
            {
                _context.CaseInsurancePayments.Add(m);
            }
            _context.SaveChanges();
            n.Id = m.Id;
        }


        public void DeleteCaseInsurancePaymentVM(int Id)
        {
            var m = _context.CaseInsurancesMaxOutOfPocket.Where(p => p.Id == Id).FirstOrDefault();
            if (m != null)
            {
                _context.CaseInsurancesMaxOutOfPocket.Remove(m);
            }
            _context.SaveChanges();
        }

        public CaseInsurancePaymentVM Transform(CaseInsurancePayment n)
        {
            var r = new CaseInsurancePaymentVM
            {
                InsurancePaymentAmount = n.Amount,
                PaymentCaseInsuranceId = n.CaseInsuranceId
            };
            if (n.CaseInsurance != null)
            {
                r.PaymentCaseInsuranceName = n.CaseInsurance.Insurance.Name;
            }
            r.Description = n.Description;
            r.Id = n.Id;
            r.PaymentDate = n.PaymentDate;
            return r;
        }


        public CaseInsurancePayment Transform(CaseInsurancePaymentVM n)
        {
            CaseInsurancePayment r;
            if (n.Id > 0)
            {
                r = _context.CaseInsurancePayments.Where(p => p.Id == n.Id).FirstOrDefault();
            }
            else
            {
                r = _context.CaseInsurancePayments.Create();
            }
            r.Amount = n.InsurancePaymentAmount;
            r.CaseInsuranceId = n.PaymentCaseInsuranceId;
            r.Description = n.Description;
            r.PaymentDate = n.PaymentDate;
            return r;
        }
        #endregion


        #region CasePaymentPlan

        public IEnumerable<CasePaymentPlanVM> GetPaymentPlans(int CaseId)
        {
            var items = _context.CasePaymentPlans
                .Where(c => c.CaseId == CaseId)
                .OrderByDescending(c => c.EndDate)
                .ToList()
                .Select(Transform);
            return items;
        }


        public CasePaymentPlanVM GetPaymentPlan(int CaseId, int CasePaymentPlanId)
        {
            var paymentPlan = _context.CasePaymentPlans.Where(c => c.Id == CasePaymentPlanId).FirstOrDefault();
            if (paymentPlan == null)
            {
                paymentPlan = _context.CasePaymentPlans.Create();
                paymentPlan.Case = _context.Cases.Where(c => c.ID == CaseId).FirstOrDefault();
                paymentPlan.CaseId = CaseId;
            }
            return Transform(paymentPlan);
        }


        public void SavePaymentPlan(CasePaymentPlanVM n)
        {
            var m = Transform(n);
            if (m.Id == 0)
            {
                _context.CasePaymentPlans.Add(m);
            }
            _context.SaveChanges();
            if (m.Active)
            {
                var aPaymentPlans = _context.CasePaymentPlans.Where(p => p.CaseId == n.PaymentPlanCaseId && p.Id != m.Id);
                foreach (var p in aPaymentPlans)
                {
                    p.Active = false;
                }
                _context.SaveChanges();
            }
            n.Id = m.Id;
        }


        public CasePaymentPlanVM Transform(CasePaymentPlan n)
        {
            var r = new CasePaymentPlanVM
            {
                PaymentPlanActive = n.Active,
                Amount = n.Amount,
                PaymentPlanCaseId = n.CaseId,
                PaymentPlanEndDate = n.EndDate,
                Frequency = n.Frequency,
                Id = n.Id,
                PaymentPlanStartDate = n.StartDate,
                PatientID = n.Case.PatientID,
                PatientName = n.Case.Patient.CommonName,
                PatientGender = !string.IsNullOrEmpty(n.Case.Patient.Gender) ? ((Domain.Gender)Enum.Parse(typeof(Domain.Gender), n.Case.Patient.Gender)).ToString().First().ToString() : string.Empty
            };
            return r;
        }


        public IEnumerable<AuthResolutionDetailsDTO> GetAuthResolutionDetails(int hoursId)
        {
            return _context.Database.SqlQuery<AuthResolutionDetailsDTO>(@"
                SELECT
                    CAHBL.WasResolved, 
                    CAHBL.HoursDate,
                    CAHBL.BillableHours, 
                    CAHBL.AuthMatchRuleDetailJSON, 
                    CAHBL.ActiveAuthorizationsJSON, 
                    CAHBL.ResolvedAuthCode, 
                    CAHBL.ResolvedCaseAuthStart,
                    CAHBL.ResolvedCaseAuthEndDate,
                    CAHBL.ResolvedMinutes,
                    Services.ServiceCode, 
                    Insurances.InsuranceName, 
                    ProviderTypes.ProviderTypeCode
                FROM CaseAuthHoursBreakdownLog AS CAHBL LEFT OUTER JOIN
                    ProviderTypes ON CAHBL.ProviderTypeID = ProviderTypes.ID LEFT OUTER JOIN
                    Insurances ON CAHBL.InsuranceID = Insurances.ID LEFT OUTER JOIN
                    Services ON CAHBL.ServiceID = Services.ID
                WHERE(CAHBL.HoursID = @hoursId)
                ORDER BY CAHBL.DateCreated", new SqlParameter("@hoursId", hoursId)).ToList();
        }


        public CasePaymentPlan Transform(CasePaymentPlanVM n)
        {
            CasePaymentPlan r;
            if (n.Id > 0)
            {
                r = _context.CasePaymentPlans.Where(p => p.Id == n.Id).FirstOrDefault();
            }
            else
            {
                r = _context.CasePaymentPlans.Create();
            }
            r.Active = n.PaymentPlanActive;
            r.Amount = n.Amount;
            r.CaseId = n.PaymentPlanCaseId;
            r.EndDate = n.PaymentPlanEndDate;
            r.Frequency = n.Frequency;
            r.StartDate = n.PaymentPlanStartDate;
            return r;
        }

        #endregion


        #region CaseBillingCorrespondences

        public IEnumerable<CaseBillingCorrespondenceVM> GetCaseBillingCorrespondences(int CaseId)
        {
            var items = _context.CaseBillingCorrespondences
                .Where(c => c.CaseId == CaseId)
                .OrderByDescending(c => c.CorrespondenceDate)
                .ToList()
                .Select(Transform);
            return items;
        }


        public CaseBillingCorrespondenceVM GetCaseBillingCorrespondence(int CaseId, int CaseBillingCorrespondenceId)
        {
            var note = _context.CaseBillingCorrespondences.Where(c => c.Id == CaseBillingCorrespondenceId).FirstOrDefault();
            if (note == null)
            {
                return new CaseBillingCorrespondenceVM
                {
                    BillingCorrespondenceCaseId = CaseId,
                    CorrespondenceDate = DateTime.Now,
                    CorrespondenceTypeId = 1, // email
                    CorrespondenceTypeName = "Email",
                    StaffId = 0,
                    StaffName = string.Empty
                };
            }
            return Transform(note);
        }


        public void SaveCaseBillingCorrespondence(CaseBillingCorrespondenceVM n)
        {
            var m = Transform(n);
            if (m.Id == 0)
            {
                _context.CaseBillingCorrespondences.Add(m);
            }
            _context.SaveChanges();
            if (n.Attachments != null)
            {
                var uploadedFile = n.Attachments.First();
                var saveName = uploadedFile.FileName;
                var partialPath = "\\case\\" + n.BillingCorrespondenceCaseId + "\\correspondence\\" + m.Id;
                var saveDir = AppService.Current.Settings.UploadDirectory + partialPath;
                var fullPath = saveDir + "\\" + saveName;
                Directory.CreateDirectory(saveDir);
                using (var fstream = new FileStream(fullPath, FileMode.Create))
                {
                    fstream.Write(uploadedFile.FileBytes, 0, uploadedFile.FileBytes.Length);
                }
                m.AttachmentFilename = saveName;
                _context.SaveChanges();
            }
            n.Id = m.Id;
        }


        public CaseBillingCorrespondenceVM Transform(CaseBillingCorrespondence n)
        {
            var r = new CaseBillingCorrespondenceVM
            {
                AttachmentFilename = n.AttachmentFilename,
                BillingCorrespondenceCaseId = n.CaseId,
                ContactPerson = n.ContactPerson,
                CorrespondenceTypeId = n.CorrespondenceTypeId,
                CorrespondenceTypeName = n.CaseBillingCorrespondenceType.Name,
                StaffId = n.StaffId,
                StaffName = n.Staff.StaffFirstName + " " + n.Staff.StaffLastName,
                CorrespondenceDate = n.CorrespondenceDate,
                Id = n.Id,
                Notes = n.Notes
            };
            return r;
        }


        public CaseBillingCorrespondence Transform(CaseBillingCorrespondenceVM n)
        {
            CaseBillingCorrespondence r;
            if (n.Id > 0)
            {
                r = _context.CaseBillingCorrespondences.Where(p => p.Id == n.Id).FirstOrDefault();
            }
            else
            {
                r = _context.CaseBillingCorrespondences.Create();
            }
            r.AttachmentFilename = n.AttachmentFilename;
            r.CaseId = n.BillingCorrespondenceCaseId;
            r.ContactPerson = n.ContactPerson;
            r.CorrespondenceTypeId = n.CorrespondenceTypeId;
            r.StaffId = n.StaffId;
            r.CorrespondenceDate = n.CorrespondenceDate;
            r.Notes = n.Notes;
            return r;
        }

        #endregion


        #region CaseBillingCorrespondenceTypes

        public IEnumerable<CaseBillingCorrespondenceTypeVM> GetCaseBillingCorrespondenceTypes()
        {
            var r = _context.CaseBillingCorrespondenceTypes.ToList().Select(t => new CaseBillingCorrespondenceTypeVM
            {
                Id = t.Id,
                Name = t.Name
            });
            return r;
        }

        #endregion


        public ValidationIssueCollection BatchUpdateHours(int caseID, List<TimeBillGridListItemVM> update)
        {
            throw new NotImplementedException();
        }


        public void BatchDeleteHours(List<int> deleteKeys)
        {
            foreach (int key in deleteKeys)
            {
                HoursRemovalService.DeleteHours(key, _context);
            }
        }


        public ValidationIssueCollection AddSingleHourEntry(bool ignoreWarnings, int caseID, int providerID, DateTime date, DateTime timeIn, DateTime timeOut, int serviceID, string notes, bool isAdjustment)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var hourEntry = new Domain2.Hours.Hours
                    {
                        CaseID = caseID,
                        Provider = _context.Providers.Single(m => m.ID == providerID),
                        Service = _context.Services.Single(m => m.ID == serviceID),
                        Date = date,
                        StartTime = timeIn.TimeOfDay,
                        EndTime = timeOut.TimeOfDay,
                        Memo = notes
                    };
                    hourEntry.ProviderID = hourEntry.Provider.ID;
                    hourEntry.ServiceID = hourEntry.Service.ID;

                    var repository = new ResolutionServiceRepository(_context);
                    var resolutionService = new ResolutionService(new List<Domain2.Hours.Hours> { hourEntry }, repository)
                    {
                        EntryMode = HoursEntryMode.ManagementEntry
                    };
                    var issues = resolutionService.Resolve();
                    if (issues.HasErrors || (issues.HasWarnings && !ignoreWarnings))
                    {
                        transaction.Rollback();
                        return issues;
                    }
                    else
                    {
                        _context.SaveChanges();
                        transaction.Commit();
                        return issues;
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Exceptions.Handle(e, Global.GetWebInfo());
                    throw e;
                }
            }
        }


        public ValidationIssueCollection TimeBillBatchUpdate(bool ignoreWarnings, MVCxGridViewBatchUpdateValues<TimeBillGridListItemVM, int> updateValues)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var hourEntries = new List<Domain2.Hours.Hours>();
                    foreach (var updatedEntry in updateValues.Update)
                    {
                        var existingEntry = _context.Hours.Find(updatedEntry.HoursID);
                        existingEntry.HasCatalystData = updatedEntry.HasCatalystData;
                        existingEntry.Service = _context.Services.Single(x => x.ID == updatedEntry.Service.ID);
                        existingEntry.ServiceID = existingEntry.Service.ID;
                        existingEntry.Date = updatedEntry.Date;
                        existingEntry.StartTime = updatedEntry.TimeIn.TimeOfDay;
                        existingEntry.EndTime = updatedEntry.TimeOut.TimeOfDay;
                        existingEntry.ServiceLocationID = updatedEntry.ServiceLocationID.GetValueOrDefault(GetHomeDefault());
                        existingEntry.BillableHours = (decimal)updatedEntry.Billable;
                        existingEntry.PayableHours = (decimal)updatedEntry.Payable;
                        hourEntries.Add(existingEntry);
                    }
                    var repository = new ResolutionServiceRepository(_context);
                    var resolutionService = new ResolutionService(hourEntries, repository)
                    {
                        UseExplicitBillableValue = true,
                        UseExplicitPayableValue = true,
                        EntryMode = HoursEntryMode.ManagementEntry
                    };
                    var issues = resolutionService.Resolve();
                    if (issues.HasErrors || (issues.HasWarnings && !ignoreWarnings))
                    {
                        transaction.Rollback();
                        return issues;
                    }
                    else
                    {
                        _context.SaveChanges();
                        transaction.Commit();
                        return issues;
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Exceptions.Handle(e, Global.GetWebInfo());
                    throw e;
                }
            }
        }


        private int GetHomeDefault()
        {
            // return the id of the Home location
            return _context.ServiceLocations.Where(x => x.Name == "Home").Single().ID;
        }


        public static void MergePDFs(string targetPath, params string[] pdfs)
        {
            using (var targetDoc = new PdfDocument())
            {
                foreach (string pdf in pdfs)
                {
                    using (var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
                    {
                        for (int i = 0; i < pdfDoc.PageCount; i++)
                        {
                            targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                }
                targetDoc.Save(targetPath);
            }
        }


        public TimeBillVM GetCaseTimeBillModel(int caseID, DateTime periodStartDate)
        {
            var periodEndDate = periodStartDate.AddMonths(1).AddDays(-1);
            var model = new TimeBillVM
            {
                GridItems = GetCaseHoursByCase(caseID, periodStartDate, periodEndDate).ToList(),
                //var newHours = App.Cases.CaseService.
                HoursMatrix = DomainServices.Hours.PeriodMatrixByCase.GetPeriodMatrix(caseID, periodStartDate, periodStartDate.AddMonths(1).AddDays(-1))
            };
            return model;
        }


        public IEnumerable<TimeBillGridListItemVM> GetCaseHoursByCase(int caseID, DateTime periodStart, DateTime periodEnd)
        {
            var items = _context.Hours.Where(x => x.CaseID == caseID && x.Date >= periodStart && x.Date <= periodEnd)
                .OrderBy(x => x.Provider.ProviderTypeID)
                .ThenBy(x => x.Provider.LastName)
                .ThenBy(x => x.Date)
                .ToList().Select(hour => new TimeBillGridListItemVM
                {
                    Date = hour.Date,
                    Hours = (double)hour.TotalHours,
                    HoursID = hour.ID,
                    Notes = hour.Memo,
                    Payable = (double)hour.PayableHours,
                    ProviderID = hour.ProviderID,
                    ProviderName = CommonListItems.GetCommonName(hour.Provider.FirstName, hour.Provider.LastName),
                    ProviderTypeID = hour.Provider.ProviderTypeID,
                    ProviderTypeName = hour.Provider.GetProviderTypeFullCode(),
                    StatusCode = GetTimeBillStatusCodeDisplay(hour.Status),
                    StatusID = (Domain.Cases.AuthorizationHoursStatus)hour.Status,
                    Service = hour.Service,
                    TimeIn = hour.Date + hour.StartTime,
                    TimeOut = hour.Date + hour.EndTime,
                    Paid = hour.PayableRef == null ? false : true,
                    Billed = hour.BillingRef == null ? false : true,
                    HasCatalystData = hour.HasCatalystData,
                    Reported = hour.ParentReported,
                    PreCheckedNoSession = hour.Status == Domain2.Hours.HoursStatus.PreChecked && (string.IsNullOrEmpty(hour.Memo) && hour.ExtendedNotes.All(m => string.IsNullOrEmpty(m.Answer))),
                    ServiceLocationID = hour.ServiceLocationID,
                    AuthCode = hour.AuthorizationBreakdowns != null ? string.Join(", ", hour.AuthorizationBreakdowns.Select(x => x.Authorization?.AuthorizationCode.Code)) : string.Empty,
                    Billable = (double)hour.BillableHours,
                    CaseAuthID = hour.Authorization?.ID,
                    CaseAuth = hour.Authorization != null ? new TimeBillGridAuthItemVM
                    {
                        ID = hour.Authorization.ID,
                        Code = hour.Authorization.AuthorizationCode.Code
                    } : null
                });
            return items;
        }


        private string GetTimeBillStatusCodeDisplay(Domain2.Hours.HoursStatus status)
        {
            switch (status)
            {
                case Domain2.Hours.HoursStatus.FinalizedByProvider:
                    return "E";
                case Domain2.Hours.HoursStatus.ScrubbedByAdmin:
                    return "F";
                default:
                    return "F";
            }
        }


        public static IEnumerable<GeneralHoursBilledListItem> GetGeneralHoursBilledListItems(int caseID, DateTime startDate, DateTime endDate)
        {
            if (startDate == new DateTime(1, 1, 1))
            {
                return null;
            }
            var context = AppService.Current.DataContext;
            var results = context.Database.SqlQuery<GeneralHoursBilledListItemDTO>(
                "dbo.GetBilledHoursByMonthByProviderType @CaseID, @StartDate, @EndDate",
                new SqlParameter { ParameterName = "@CaseID", SqlDbType = SqlDbType.Int, SqlValue = caseID },
                new SqlParameter { ParameterName = "@StartDate", SqlDbType = SqlDbType.Date, SqlValue = startDate },
                new SqlParameter { ParameterName = "@EndDate", SqlDbType = SqlDbType.Date, SqlValue = endDate }
                ).ToList();
            var items = results
                .OrderBy(x => x.MonthStart)
                .Select(res => new GeneralHoursBilledListItem
                {
                    Month = res.MonthStart.ToString("MMM"),
                    AideHours = res.AideHours,
                    BCBAHours = res.BCBAHours,
                    TotalHours = res.TotalHours
                });
            return items;
        }


        class GeneralHoursBilledListItemDTO
        {
            public DateTime MonthStart { get; set; }
            public decimal? BCBAHours { get; set; }
            public decimal? AideHours { get; set; }
            public decimal? TotalHours { get; set; }
        }

        private static class Mapper
        {
            public static CaseInsuranceHistoryDTO ToInsuranceHistoryDTO(CaseInsurance i)
            {
                var dto = new CaseInsuranceHistoryDTO
                {
                    Id = i.ID,
                    MemberId = i.MemberId,
                    DatePlanEffective = i.DatePlanEffective,
                    DatePlanTerminated = i.DatePlanTerminated,
                    OtherNotes = i.OtherNotes,
                    InsuranceName = i.Insurance != null ? i.Insurance.Name + (i.CarrierID.HasValue ? " (" + i.Carrier.Name + ")" : string.Empty) : string.Empty
                };
                return dto;
            }


            public static CaseInsuranceDTO ToInsuranceDTO(CaseInsurance caseInsurance)
            {
                var dto = new CaseInsuranceDTO
                {
                    Id = caseInsurance.ID,
                    InsuranceCaseId = caseInsurance.CaseID,
                    InsuranceId = caseInsurance.InsuranceID,
                    InsuranceCarrierId = caseInsurance.CarrierID,
                    MemberName = caseInsurance.MemberName,
                    MemberId = caseInsurance.MemberId,
                    PrimaryCardholderName = caseInsurance.PrimaryCardholderName,
                    DatePlanEffective = caseInsurance.DatePlanEffective,
                    DatePlanTerminated = caseInsurance.DatePlanTerminated,
                    HardshipWaiverApplied = caseInsurance.HardshipWaiverApplied,
                    HardshipWaiverApproved = caseInsurance.HardshipWaiverApproved,
                    HardshipWaiverLike = caseInsurance.HardshipWaiverLike,
                    OtherNotes = caseInsurance.OtherNotes,
                    FundingType = caseInsurance.FundingType,
                    BenefitType = caseInsurance.BenefitType,
                    CoInsuranceAmount = caseInsurance.CoInsuranceAmount,
                    CoPayAmount = caseInsurance.CoPayAmount,
                    DeductibleTotal = caseInsurance.DeductibleTotal,
                    MaxOutOfPocket = caseInsurance.CaseInsurancesMaxOutOfPocket
                    .OrderByDescending(c => c.EffectiveDate)
                    .ToList()
                    .Select(m => ToCaseInsuranceMaxOutOfPocketDTO(m))
                };
                return dto;
            }


            public static void ToCaseInsurance(CaseInsuranceDTO dto, CaseInsurance caseInsurance)
            {
                caseInsurance.InsuranceID = dto.InsuranceId;
                caseInsurance.CarrierID = dto.InsuranceCarrierId;
                caseInsurance.MemberName = dto.MemberName;
                caseInsurance.MemberId = dto.MemberId;
                caseInsurance.PrimaryCardholderName = dto.PrimaryCardholderName;
                caseInsurance.DatePlanEffective = dto.DatePlanEffective;
                caseInsurance.DatePlanTerminated = dto.DatePlanTerminated;
                caseInsurance.HardshipWaiverApplied = dto.HardshipWaiverApplied;
                caseInsurance.HardshipWaiverApproved = dto.HardshipWaiverApproved;
                caseInsurance.HardshipWaiverLike = dto.HardshipWaiverLike;
                caseInsurance.OtherNotes = dto.OtherNotes;
                caseInsurance.FundingType = dto.FundingType;
                caseInsurance.BenefitType = dto.BenefitType;
                caseInsurance.CoInsuranceAmount = dto.CoInsuranceAmount;
                caseInsurance.CoPayAmount = dto.CoPayAmount;
                caseInsurance.DeductibleTotal = dto.DeductibleTotal;
            }


            public static CaseInsuranceMaxOutOfPocketDTO ToCaseInsuranceMaxOutOfPocketDTO(CaseInsuranceMaxOutOfPocket n)
            {
                var dto = new CaseInsuranceMaxOutOfPocketDTO
                {
                    Id = n.Id,
                    CaseInsuranceId = n.CaseInsuranceId,
                    EffectiveDate = n.EffectiveDate,
                    MaxOutOfPocket = n.MaxOutOfPocket
                };
                return dto;
            }


            public static void ToCaseInsuranceMaxOutOfPocket(CaseInsuranceMaxOutOfPocketDTO dto, CaseInsuranceMaxOutOfPocket r)
            {
                r.EffectiveDate = dto.EffectiveDate;
                r.MaxOutOfPocket = dto.MaxOutOfPocket;
            }

        }

    }
}
