using AABC.Data.V2;
using AABC.Web.App.Patients;
using AABC.Web.App.Patients.Models;
using AABC.Web.Models.Patients;
using Dymeng.Framework;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.Repositories
{

    public interface IPatientRepository
    {
        PatientVM GetPatient();
        PatientVM GetPatient(int id);
        IEnumerable<PatientsListItem2VM> GetPatientListItems();
        IEnumerable<PatientsListItem2VM> GetDischargedPatientListItems();
        int? GetPatientByCase(int caseID);
        void SetRestaffReasonID(int caseID, int reasonID);
        void SavePatient(PatientVM patient);
        void DeletePatient(int id);
        bool DuplicateExists(string firstName, string lastName, DateTime dateOfBirth);
        int GetCaseIDByPatientID(int patientID);
        void UpdateHasPrescription(int caseID, bool hasRx);
        void UpdateHasIntake(int caseID, bool hasIntake);
        void UpdateStartDate(int caseID, DateTime? date);
        void RecalculateCaseStatus(int caseID);
        List<Domain.General.GuardianRelationship> GetGuardianRelationships();
        List<InsuranceListItem> GetInsuranceList();
        void UpdateNeedsRestaffing(int caseID, bool newValue);
        List<RestaffReasonListItem> GetRestaffReasonItems();
        int? GetRestaffReason(int id);
    }


    public class PatientRepository : IPatientRepository
    {

        public void SetRestaffReasonID(int caseID, int reasonID)
        {
            var c = Context.Cases.Find(caseID);
            if (reasonID == 0)
            {
                c.RestaffReasonID = null;
            }
            else
            {
                c.RestaffReasonID = reasonID;
                //if (c.NeedsRestaffing && (c.RestaffReasonID == (int)Domain2.Cases.RestaffReason.NewNeedsBCBA || c.RestaffReasonID == (int)Domain2.Cases.RestaffReason.NewNeedsAide))
                //{
                //    SaveNewStaffingLog(c.PatientID);
                //}
            }
            Context.SaveChanges();
            PatientSearchService.UpdateEntry(c.PatientID);
            DomainServices.Staffing.StaffingService ss = new DomainServices.Staffing.StaffingService(Context);
            ss.PerformCheckByCaseId(caseID);
        }


        public List<RestaffReasonListItem> GetRestaffReasonItems()
        {
            var items = new List<RestaffReasonListItem>();
            var data = Domain2.Cases.Case.GetRestaffReasonsList();
            foreach (var t in data)
            {
                items.Add(new RestaffReasonListItem() { ID = (int)t.Item1, Reason = t.Item2 });
            }
            return items;
        }


        public int? GetRestaffReason(int id)
        {
            var c = Context.Cases.Find(id);
            return c.RestaffReasonID;
        }


        public List<InsuranceListItem> GetInsuranceList()
        {
            var insurances = Context.Insurances.OrderBy(x => x.Name).ToList();
            var items = new List<InsuranceListItem>();
            foreach (var ins in insurances)
            {
                items.Add(new InsuranceListItem()
                {
                    ID = ins.ID,
                    Name = ins.Name
                });
            }
            return items;
        }


        public void UpdateStartDate(int caseID, DateTime? date)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Cases SET CaseStartDate = @Date WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", caseID);
                cmd.Parameters.AddWithNullableValue("@Date", date);
                cmd.ExecuteNonQueryToInt();
                PatientSearchService.UpdateEntry(Context.Cases.Find(caseID).PatientID);
                DomainServices.Staffing.StaffingService ss = new DomainServices.Staffing.StaffingService(Context);
                ss.PerformCheckByCaseId(caseID);
            }
        }


        public void UpdateNeedsRestaffing(int caseID, bool needsRestaffing)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Cases SET CaseNeedsRestaffing = 1 - CaseNeedsRestaffing WHERE ID = @ID;";
                //cmd.Parameters.AddWithValue("@V", needsRestaffing);
                cmd.Parameters.AddWithValue("@ID", caseID);
                cmd.ExecuteNonQueryToInt();
                PatientSearchService.UpdateEntry(Context.Cases.Find(caseID).PatientID);
                DomainServices.Staffing.StaffingService ss = new DomainServices.Staffing.StaffingService(Context);
                ss.PerformCheckByCaseId(caseID);
            }
        }


        //public void SaveNewStaffingLog(int PatientID)
        //{
        //    using (SqlConnection conn = new SqlConnection(this.connectionString))
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        cmd.Connection = conn;
        //        cmd.CommandText = "if not exists (select 1 from StaffingLog where PatientID=@ID) insert into StaffingLog (PatientID) values (@ID)";
        //        cmd.Parameters.AddWithValue("@ID", PatientID);
        //        cmd.ExecuteNonQueryToInt();
        //    }
        //}


        public void UpdateHasPrescription(int caseID, bool hasRx)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Cases SET CaseHasPrescription = @Rx WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@Rx", hasRx);
                cmd.Parameters.AddWithValue("@ID", caseID);
                cmd.ExecuteNonQueryToInt();
                RecalculateCaseStatus(caseID);
                PatientSearchService.UpdateEntry(Context.Cases.Find(caseID).PatientID);
            }
        }


        public void UpdateHasIntake(int caseID, bool hasIntake)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Cases SET CaseHasIntake = @Intake WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@Intake", hasIntake);
                cmd.Parameters.AddWithValue("@ID", caseID);
                cmd.ExecuteNonQueryToInt();
                RecalculateCaseStatus(caseID);
                PatientSearchService.UpdateEntry(Context.Cases.Find(caseID).PatientID);
            }
        }


        public void RecalculateCaseStatus(int caseID)
        {
            Domain.Cases.Case c = new Domain.Cases.Case();
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string sqlCase = "";
                sqlCase += "SELECT c.CaseStatus, c.CaseStartDate, c.CaseStatusReason, ";
                sqlCase += "  c.CaseHasPrescription, c.CaseHasIntake ";
                sqlCase += "FROM dbo.Cases AS c ";
                sqlCase += "WHERE c.ID = @CaseID ";
                sqlCase += ";";
                string sqlProviders = "";
                sqlProviders += "SELECT cp.Active, cp.IsSupervisor ";
                sqlProviders += "FROM dbo.CaseProviders AS cp ";
                sqlProviders += "WHERE cp.CaseID = @CaseID ";
                sqlProviders += ";";
                string sqlAuths = "";
                sqlAuths += "SELECT ca.AuthEndDate ";
                sqlAuths += "FROM dbo.CaseAuthCodes AS ca ";
                sqlAuths += "WHERE ca.CaseID = @CaseID ";
                sqlAuths += ";";
                cmd.CommandText = sqlCase + sqlProviders + sqlAuths;
                cmd.Parameters.AddWithValue("@CaseID", caseID);

                var set = cmd.GetDataSet(new string[] { "Case", "Providers", "Auths" });
                DataRow r = set.Tables["Case"].Rows[0];
                c.Status = (Domain.Cases.CaseStatus)r.ToInt("CaseStatus");
                c.StatusReason = (Domain.Cases.CaseStatusReason)r.ToInt("CaseStatusReason");
                c.StartDate = r.ToDateTimeOrNull("CaseStartDate");
                c.HasPrescription = r.ToBool("CaseHasPrescription");
                c.HasIntake = r.ToBool("CaseHasIntake");
                c.Providers = new List<Domain.Cases.CaseProvider>();
                for (int i = 0; i < set.Tables["Providers"].Rows.Count; i++)
                {
                    var p = new Domain.Cases.CaseProvider();
                    r = set.Tables["Providers"].Rows[i];
                    p.Active = r.ToBool("Active");
                    p.Supervisor = r.ToBool("IsSupervisor");
                    c.Providers.Add(p);
                }
                c.Authorizations = new List<Domain.Cases.CaseAuthorization>();
                for (int i = 0; i < set.Tables["Auths"].Rows.Count; i++)
                {
                    var auth = new Domain.Cases.CaseAuthorization();
                    r = set.Tables["Auths"].Rows[i];
                    auth.EndDate = r.ToDateTime("AuthEndDate");
                    c.Authorizations.Add(auth);
                }
                c.CalculateStatus();
                SaveCaseStatus(caseID, c.Status, c.StatusReason);
            }
        }


        private void SaveCaseStatus(int caseID, Domain.Cases.CaseStatus status, Domain.Cases.CaseStatusReason statusReason)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Cases SET CaseStatus = @Status, CaseStatusReason = @StatusReason WHERE ID = @CaseID;";
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@StatusReason", statusReason);
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.ExecuteNonQueryToInt();
                //PatientSearchService.UpdateEntry(Context.Cases.Find(caseID).PatientID);
                //above line is handled by callers
            }
        }


        public int GetCaseIDByPatientID(int patientID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ID FROM dbo.Cases WHERE PatientID = @PatientID;";
                cmd.Parameters.AddWithValue("@PatientID", patientID);
                DataRow r = cmd.GetRow();
                return r.ToInt("ID");
            }
        }


        public int? GetPatientByCase(int caseID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT PatientID FROM dbo.Cases WHERE ID = @CaseID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                try
                {
                    DataRow row = cmd.GetRowOrNull();
                    if (row == null)
                    {
                        return null;
                    }
                    else
                    {
                        return row.ToIntOrNull("PatientID");
                    }
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        public IEnumerable<PatientsListItem2VM> GetDischargedPatientListItems()
        {
            return PatientSearchService.GetAll().Where(m => m.Status == Domain.Cases.CaseStatus.History);            
        }


        public IEnumerable<PatientsListItem2VM> GetPatientListItems()
        {
            return PatientSearchService.GetAll().Where(m => m.Status != Domain.Cases.CaseStatus.History);
        }


        public void DeletePatient(int caseID)
        {
            int patientID = GetPatientByCase(caseID).Value;
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "DELETE FROM dbo.Cases WHERE ID = @CaseID;" +
                    "DELETE FROM dbo.Patients WHERE ID = @PatientID;";
                cmd.Parameters.AddWithValue("@CaseID", caseID);
                cmd.Parameters.AddWithValue("@PatientID", patientID);
                cmd.ExecuteNonQueryToInt();
                PatientSearchService.Remove(patientID);
            }
        }


        public PatientVM GetPatient()
        {
            return new PatientVM();
        }


        public PatientVM GetPatient(int id)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT " +
                    "  PatientGeneratingReferralID, PatientFirstName, PatientLastName, PatientDateofBirth, " +
                    "  PatientGender, PatientPrimarySpokenLangauge, " +
                    "  PatientGuardianRelationship, PatientEmail, PatientPhone, PatientAddress1, PatientAddress2, " +
                    "  PatientCity, PatientState, PatientZip, PatientInsuranceCompanyName, PatientInsuranceMemberID, " +
                    "  PatientInsurancePrimaryCardholderDateofBirth, PatientInsuranceCompanyProviderPhone, " +
                    "  PatientGuardianFirstName, PatientGuardianLastName, PatientGuardianRelationshipID, " +
                    "  PatientGuardianEmail, PatientGuardianCellPhone, PatientGuardianHomePhone, PatientGuardianWorkPhone, " +
                    "  PatientGuardianNotes, " +
                    "  PatientGuardian2FirstName, PatientGuardian2LastName, PatientGuardian2RelationshipID, " +
                    "  PatientGuardian2Email, PatientGuardian2CellPhone, PatientGuardian2HomePhone, PatientGuardian2WorkPhone, " +
                    "  PatientGuardian2Notes, " +
                    "  PatientGuardian3FirstName, PatientGuardian3LastName, PatientGuardian3RelationshipID, " +
                    "  PatientGuardian3Email, PatientGuardian3CellPhone, PatientGuardian3HomePhone, PatientGuardian3WorkPhone, " +
                    "  PatientGuardian3Notes, " +
                    "  PatientNotes, PatientInsuranceID, " +
                    "  PatientPhysicianName, PatientPhysicianAddress, PatientPhysicianPhone, PatientPhysicianFax, " +
                    "  PatientPhysicianEmail, PatientPhysicianContact, PatientPhysicianNotes, " +
                    "  PrescriptionFileName, PrescriptionLocation, HighRisk " +
                    "FROM dbo.Patients " +
                    "WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", id);
                try
                {
                    var r = cmd.GetRowOrNull();
                    var p = new PatientVM();
                    if (r == null)
                    {
                        return p;
                    }
                    Data.Services.CaseService caseService = new Data.Services.CaseService();
                    var guardianRelationships = caseService.GetGuardianRelationships();
                    p.ID = id;
                    p.GeneratingReferralID = r.ToIntOrNull(0);
                    p.FirstName = r.ToStringValue(1);
                    p.LastName = r.ToStringValue(2);
                    p.DateOfBirth = r.ToDateTimeOrNull(3);
                    p.Gender = !string.IsNullOrEmpty(r.ToStringValue(4)) ? (Domain.Gender?)Enum.Parse(typeof(Domain.Gender), r.ToStringValue(4)) : null;
                    p.Language = new Domain.Referrals.Language() { Code = r.ToStringValue(5) };
                    p.GuardianRelationship = r.ToStringValue(6);
                    p.Email = r.ToStringValue(7);
                    p.Phone = r.ToStringValue(8);
                    p.Address1 = r.ToStringValue(9);
                    p.Address2 = r.ToStringValue(10);
                    p.City = r.ToStringValue(11);
                    p.State = r.ToStringValue(12);
                    p.Zip = r.ToStringValue(13);
                    p.InsuranceCompanyName = r.ToStringValue(14);
                    p.InsuranceMemberID = r.ToStringValue(15);
                    p.InsurancePrimaryCardholderDOB = r.ToDateTimeOrNull(16);
                    p.InsuranceProviderPhone = r.ToStringValue(17);
                    p.GuardianFirstName = r.ToStringValue(18);
                    p.GuardianLastName = r.ToStringValue(19);
                    p.GuardianRelationshipObject = GetGuardianRelationship(r.ToIntOrNull(20), guardianRelationships);
                    p.GuardianEmail = r.ToStringValue(21);
                    p.GuardianCellPhone = r.ToStringValue(22);
                    p.GuardianHomePhone = r.ToStringValue(23);
                    p.GuardianWorkPhone = r.ToStringValue(24);
                    p.GuardianNotes = r.ToStringValue(25);
                    p.Guardian2FirstName = r.ToStringValue(26);
                    p.Guardian2LastName = r.ToStringValue(27);
                    p.Guardian2RelationshipObject = GetGuardianRelationship(r.ToIntOrNull(28), guardianRelationships);
                    p.Guardian2Email = r.ToStringValue(29);
                    p.Guardian2CellPhone = r.ToStringValue(30);
                    p.Guardian2HomePhone = r.ToStringValue(31);
                    p.Guardian2WorkPhone = r.ToStringValue(32);
                    p.Guardian2Notes = r.ToStringValue(33);
                    p.Guardian3FirstName = r.ToStringValue(34);
                    p.Guardian3LastName = r.ToStringValue(35);
                    p.Guardian3RelationshipObject = GetGuardianRelationship(r.ToIntOrNull(36), guardianRelationships);
                    p.Guardian3Email = r.ToStringValue(37);
                    p.Guardian3CellPhone = r.ToStringValue(38);
                    p.Guardian3HomePhone = r.ToStringValue(39);
                    p.Guardian3WorkPhone = r.ToStringValue(40);
                    p.Guardian3Notes = r.ToStringValue(41);
                    p.Notes = r.ToStringValue(42);
                    p.InsuranceID = r.ToIntOrNull(43);
                    p.PhysicianName = r.ToStringValue(44);
                    p.PhysicianAddress = r.ToStringValue(45);
                    p.PhysicianPhone = r.ToStringValue(46);
                    p.PhysicianFax = r.ToStringValue(47);
                    p.PhysicianEmail = r.ToStringValue(48);
                    p.PhysicianContact = r.ToStringValue(49);
                    p.PhysicianNotes = r.ToStringValue(50);
                    p.PrescriptionFileName = r.ToStringValue(51);
                    p.PrescriptionLocation = r.ToStringValue(52);
                    p.HighRisk = r.ToBool(53);
                    return p;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        public bool DuplicateExists(string firstName, string lastName, DateTime dateOfBirth)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "Select ID from dbo.Patients " +
                    "Where PatientFirstName = @firstName " +
                    "And PatientLastName = @lastName " +
                    "And PatientDateofBirth = @dateOfBirth ";
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@dateOfBirth", dateOfBirth);
                try
                {
                    var row = cmd.GetRowOrNull();
                    if (row == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        Domain.General.GuardianRelationship GetGuardianRelationship(int? id, List<Domain.General.GuardianRelationship> items)
        {
            if (!id.HasValue)
            {
                return null;
            }
            else
            {
                return items.Where(x => x.ID == id.Value).FirstOrDefault();
            }
        }


        public void SavePatient(PatientVM patient)
        {
            if (patient.ID.HasValue)
            {
                UpdateExistingPatient(patient);
            }
            else
            {
                SaveNewPatient(patient);
            }
            SavePrescriptionFile(patient);
        }


        void SavePrescriptionFile(PatientVM p)
        {
            if (p.PrescriptionFile.Count() == 0) { return; }
            if (p.PrescriptionFile.Count() == 1 && p.PrescriptionFile.First().FileName == "") { return; }
            string oldLocation = "";
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string sql = @"
                    SELECT [PrescriptionLocation]
                    FROM dbo.Patients
                    WHERE ID = @ID;";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ID", p.ID);
                try
                {
                    cmd.Connection.Open();
                    var result = cmd.ExecuteScalar();
                    if (!(result is DBNull))
                    {
                        oldLocation = (string)result;
                    }
                    cmd.Connection.Close();
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
            if (!String.IsNullOrEmpty(oldLocation))
            {
                System.IO.File.Delete(AppService.Current.Settings.UploadDirectory + oldLocation);
            }

            var uploadedFile = p.PrescriptionFile.First();
            string saveName = p.ID + uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));
            string partialPath = "\\patient_prescriptions\\" + p.LastName.Substring(0, 1).ToUpper();
            string saveDir = AppService.Current.Settings.UploadDirectory + partialPath;
            string fullPath = saveDir + "\\" + saveName;
            System.IO.Directory.CreateDirectory(saveDir);
            using (var fstream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
            {
                fstream.Write(uploadedFile.FileBytes, 0, uploadedFile.FileBytes.Length);
            }

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                string sql = @"
                    UPDATE dbo.Patients SET
                    [PrescriptionFileName] = @FileName,
                    [PrescriptionLocation] = @Location
                    WHERE ID = @ID;";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@FileName", uploadedFile.FileName);
                cmd.Parameters.AddWithValue("@Location", partialPath + "\\" + saveName);
                cmd.Parameters.AddWithValue("@ID", p.ID);
                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        void UpdateExistingPatient(PatientVM p)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "UPDATE dbo.Patients SET " +
                    "  PatientGeneratingReferralID = @, " +
                    "  PatientFirstName = @, " +
                    "  PatientLastName = @, " +
                    "  PatientDateOfBirth = @, " +
                    "  PatientGender = @, " +
                    "  PatientPrimarySpokenLangauge = @, " +
                    "  PatientGuardianRelationship = @, " +
                    "  PatientEmail = @, " +
                    "  PatientPhone = @, " +
                    "  PatientAddress1 = @, " +
                    "  PatientAddress2 = @, " +
                    "  PatientCity = @, " +
                    "  PatientState = @, " +
                    "  PatientZip = @, " +
                    "  PatientInsuranceCompanyName = @, " +
                    "  PatientInsuranceMemberID = @, " +
                    "  PatientInsurancePrimaryCardholderDateOfBirth = @, " +
                    "  PatientInsuranceCompanyProviderPhone = @, " +
                    "  PatientGuardianFirstName = @, " +
                    "  PatientGuardianLastName = @, " +
                    "  PatientGuardianRelationshipID = @, " +
                    "  PatientGuardianEmail = @, " +
                    "  PatientGuardianCellPhone = @, " +
                    "  PatientGuardianHomePhone = @, " +
                    "  PatientGuardianWorkPhone = @, " +
                    "  PatientGuardianNotes = @, " +
                    "  PatientGuardian2FirstName = @, " +
                    "  PatientGuardian2LastName = @, " +
                    "  PatientGuardian2RelationshipID = @, " +
                    "  PatientGuardian2Email = @, " +
                    "  PatientGuardian2CellPhone = @, " +
                    "  PatientGuardian2HomePhone = @, " +
                    "  PatientGuardian2WorkPhone = @, " +
                    "  PatientGuardian2Notes = @, " +
                    "  PatientGuardian3FirstName = @, " +
                    "  PatientGuardian3LastName = @, " +
                    "  PatientGuardian3RelationshipID = @, " +
                    "  PatientGuardian3Email = @, " +
                    "  PatientGuardian3CellPhone = @, " +
                    "  PatientGuardian3HomePhone = @, " +
                    "  PatientGuardian3WorkPhone = @, " +
                    "  PatientGuardian3Notes = @, " +
                    "  PatientNotes = @, " +
                    "  PatientInsuranceID = @, " +
                    "  PatientPhysicianName = @, " +
                    "  PatientPhysicianAddress = @, " +
                    "  PatientPhysicianPhone = @, " +
                    "  PatientPhysicianFax = @, " +
                    "  PatientPhysicianEmail = @, " +
                    "  PatientPhysicianContact = @, " +
                    "  PatientPhysicianNotes = @, " +
                    "  HighRisk = @ " +
                    "WHERE ID = @ID";
                ParameterInfo[] pi = new ParameterInfo[52];
                pi[0] = new ParameterInfo() { Value = p.GeneratingReferralID, Nullable = true };
                pi[1] = new ParameterInfo() { Value = p.FirstName, Nullable = false };
                pi[2] = new ParameterInfo() { Value = p.LastName, Nullable = false };
                pi[3] = new ParameterInfo() { Value = p.DateOfBirth, Nullable = true };
                pi[4] = new ParameterInfo() { Value = p.Gender, Nullable = true };
                //pi[5] = new ParameterInfo() { Value = p.Language.Code, Nullable = true };
                //pi[4] = new ParameterInfo() { Value = null, Nullable = true };
                pi[5] = new ParameterInfo() { Value = null, Nullable = true };
                pi[6] = new ParameterInfo() { Value = p.GuardianRelationship, Nullable = true };
                pi[7] = new ParameterInfo() { Value = p.Email, Nullable = true };
                pi[8] = new ParameterInfo() { Value = p.Phone, Nullable = true };
                pi[9] = new ParameterInfo() { Value = p.Address1, Nullable = true };
                pi[10] = new ParameterInfo() { Value = p.Address2, Nullable = true };
                pi[11] = new ParameterInfo() { Value = p.City, Nullable = true };
                pi[12] = new ParameterInfo() { Value = p.State, Nullable = true };
                pi[13] = new ParameterInfo() { Value = p.Zip, Nullable = true };
                pi[14] = new ParameterInfo() { Value = p.InsuranceCompanyName, Nullable = true };
                pi[15] = new ParameterInfo() { Value = p.InsuranceMemberID, Nullable = true };
                pi[16] = new ParameterInfo() { Value = p.InsurancePrimaryCardholderDOB, Nullable = true };
                pi[17] = new ParameterInfo() { Value = p.InsuranceProviderPhone, Nullable = true };
                pi[18] = new ParameterInfo() { Value = p.GuardianFirstName, Nullable = true };
                pi[19] = new ParameterInfo() { Value = p.GuardianLastName, Nullable = true };
                if (p.GuardianRelationshipObject == null)
                {
                    pi[20] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[20] = new ParameterInfo() { Value = p.GuardianRelationshipObject.ID, Nullable = true };
                }
                pi[21] = new ParameterInfo() { Value = p.GuardianEmail, Nullable = true };
                pi[22] = new ParameterInfo() { Value = p.GuardianCellPhone, Nullable = true };
                pi[23] = new ParameterInfo() { Value = p.GuardianHomePhone, Nullable = true };
                pi[24] = new ParameterInfo() { Value = p.GuardianWorkPhone, Nullable = true };
                pi[25] = new ParameterInfo() { Value = p.GuardianNotes, Nullable = true };
                pi[26] = new ParameterInfo() { Value = p.Guardian2FirstName, Nullable = true };
                pi[27] = new ParameterInfo() { Value = p.Guardian2LastName, Nullable = true };
                if (p.Guardian2RelationshipObject == null)
                {
                    pi[28] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[28] = new ParameterInfo() { Value = p.Guardian2RelationshipObject.ID, Nullable = true };
                }
                pi[29] = new ParameterInfo() { Value = p.Guardian2Email, Nullable = true };
                pi[30] = new ParameterInfo() { Value = p.Guardian2CellPhone, Nullable = true };
                pi[31] = new ParameterInfo() { Value = p.Guardian2HomePhone, Nullable = true };
                pi[32] = new ParameterInfo() { Value = p.Guardian2WorkPhone, Nullable = true };
                pi[33] = new ParameterInfo() { Value = p.Guardian2Notes, Nullable = true };
                pi[34] = new ParameterInfo() { Value = p.Guardian3FirstName, Nullable = true };
                pi[35] = new ParameterInfo() { Value = p.Guardian3LastName, Nullable = true };
                if (p.Guardian3RelationshipObject == null)
                {
                    pi[36] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[36] = new ParameterInfo() { Value = p.Guardian3RelationshipObject.ID, Nullable = true };
                }
                pi[37] = new ParameterInfo() { Value = p.Guardian3Email, Nullable = true };
                pi[38] = new ParameterInfo() { Value = p.Guardian3CellPhone, Nullable = true };
                pi[39] = new ParameterInfo() { Value = p.Guardian3HomePhone, Nullable = true };
                pi[40] = new ParameterInfo() { Value = p.Guardian3WorkPhone, Nullable = true };
                pi[41] = new ParameterInfo() { Value = p.Guardian3Notes, Nullable = true };
                pi[42] = new ParameterInfo() { Value = p.Notes, Nullable = true };
                if (p.InsuranceID.HasValue)
                {
                    pi[43] = new ParameterInfo { Value = p.InsuranceID, Nullable = true };
                }
                else
                {
                    pi[43] = new ParameterInfo { Value = null, Nullable = true };
                }
                pi[44] = new ParameterInfo { Value = p.PhysicianName, Nullable = true };
                pi[45] = new ParameterInfo { Value = p.PhysicianAddress, Nullable = true };
                pi[46] = new ParameterInfo { Value = p.PhysicianPhone, Nullable = true };
                pi[47] = new ParameterInfo { Value = p.PhysicianFax, Nullable = true };
                pi[48] = new ParameterInfo { Value = p.PhysicianEmail, Nullable = true };
                pi[49] = new ParameterInfo { Value = p.PhysicianContact, Nullable = true };
                pi[50] = new ParameterInfo { Value = p.PhysicianNotes, Nullable = true };
                pi[51] = new ParameterInfo { Value = p.HighRisk, Nullable = true };
                try
                {
                    cmd.AddParameters(pi);
                    cmd.Parameters.AddWithValue("@ID", p.ID.Value);
                    cmd.ExecuteNonQueryToInt();
                    PatientSearchService.UpdateEntry(p.ID.Value);
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        public List<Domain.General.GuardianRelationship> GetGuardianRelationships()
        {
            Data.Services.CaseService caseService = new Data.Services.CaseService();
            return caseService.GetGuardianRelationships();
        }


        void SaveNewPatient(PatientVM p)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "INSERT INTO dbo.Patients (" +
                    "  PatientGeneratingReferralID, PatientFirstName, PatientLastName, PatientDateofBirth, " +
                    "  PatientGender, PatientPrimarySpokenLangauge, " +
                    "  PatientGuardianRelationship, PatientEmail, PatientPhone, PatientAddress1, PatientAddress2, " +
                    "  PatientCity, PatientState, PatientZip, PatientInsuranceCompanyName, PatientInsuranceMemberID, " +
                    "  PatientInsurancePrimaryCardholderDateofBirth, PatientInsuranceCompanyProviderPhone, " +
                    "  PatientGuardianFirstName, PatientGuardianLastName, PatientGuardianRelationshipID, " +
                    "  PatientGuardianEmail, PatientGuardianCellPhone, PatientGuardianHomePhone, PatientGuardianWorkPhone, " +
                    "  PatientGuardianNotes, " +
                    "  PatientGuardian2FirstName, PatientGuardian2LastName, PatientGuardian2RelationshipID, " +
                    "  PatientGuardian2Email, PatientGuardian2CellPhone, PatientGuardian2HomePhone, PatientGuardian2WorkPhone, " +
                    "  PatientGuardian2Notes, " +
                    "  PatientGuardian3FirstName, PatientGuardian3LastName, PatientGuardian3RelationshipID, " +
                    "  PatientGuardian3Email, PatientGuardian3CellPhone, PatientGuardian3HomePhone, PatientGuardian3WorkPhone, " +
                    "  PatientGuardian3Notes, " +
                    "  PatientNotes, PatientInsuranceID, " +
                    "  PatientPhysicianName, PatientPhysicianAddress, PatientPhysicianPhone, PatientPhysicianFax, " +
                    "  PatientPhysicianEmail, PatientPhysicianContact, PatientPhysicianNotes, HighRisk " +
                    ") VALUES (@[ALLPARAMS]);";
                ParameterInfo[] pi = new ParameterInfo[52];
                pi[0] = new ParameterInfo() { Value = p.GeneratingReferralID, Nullable = true };
                pi[1] = new ParameterInfo() { Value = p.FirstName, Nullable = false };
                pi[2] = new ParameterInfo() { Value = p.LastName, Nullable = false };
                pi[3] = new ParameterInfo() { Value = p.DateOfBirth, Nullable = true };
                pi[4] = new ParameterInfo() { Value = p.Gender, Nullable = true };
                if (p.Language == null)
                {
                    pi[5] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[5] = new ParameterInfo() { Value = p.Language.Code, Nullable = true };
                }
                pi[4] = new ParameterInfo() { Value = null, Nullable = true };
                pi[5] = new ParameterInfo() { Value = null, Nullable = true };
                pi[6] = new ParameterInfo() { Value = p.GuardianRelationship, Nullable = true };
                pi[7] = new ParameterInfo() { Value = p.Email, Nullable = true };
                pi[8] = new ParameterInfo() { Value = p.Phone, Nullable = true };
                pi[9] = new ParameterInfo() { Value = p.Address1, Nullable = true };
                pi[10] = new ParameterInfo() { Value = p.Address2, Nullable = true };
                pi[11] = new ParameterInfo() { Value = p.City, Nullable = true };
                pi[12] = new ParameterInfo() { Value = p.State, Nullable = true };
                pi[13] = new ParameterInfo() { Value = p.Zip, Nullable = true };
                pi[14] = new ParameterInfo() { Value = p.InsuranceCompanyName, Nullable = true };
                pi[15] = new ParameterInfo() { Value = p.InsuranceMemberID, Nullable = true };
                pi[16] = new ParameterInfo() { Value = p.InsurancePrimaryCardholderDOB, Nullable = true };
                pi[17] = new ParameterInfo() { Value = p.InsuranceProviderPhone, Nullable = true };
                pi[18] = new ParameterInfo() { Value = p.GuardianFirstName, Nullable = true };
                pi[19] = new ParameterInfo() { Value = p.GuardianLastName, Nullable = true };
                if (p.GuardianRelationshipObject == null)
                {
                    pi[20] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[20] = new ParameterInfo() { Value = p.GuardianRelationshipObject.ID, Nullable = true };
                }
                pi[21] = new ParameterInfo() { Value = p.GuardianEmail, Nullable = true };
                pi[22] = new ParameterInfo() { Value = p.GuardianCellPhone, Nullable = true };
                pi[23] = new ParameterInfo() { Value = p.GuardianHomePhone, Nullable = true };
                pi[24] = new ParameterInfo() { Value = p.GuardianWorkPhone, Nullable = true };
                pi[25] = new ParameterInfo() { Value = p.GuardianNotes, Nullable = true };
                pi[26] = new ParameterInfo() { Value = p.Guardian2FirstName, Nullable = true };
                pi[27] = new ParameterInfo() { Value = p.Guardian2LastName, Nullable = true };
                if (p.Guardian2RelationshipObject == null)
                {
                    pi[28] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[28] = new ParameterInfo() { Value = p.Guardian2RelationshipObject.ID, Nullable = true };
                }
                pi[29] = new ParameterInfo() { Value = p.Guardian2Email, Nullable = true };
                pi[30] = new ParameterInfo() { Value = p.Guardian2CellPhone, Nullable = true };
                pi[31] = new ParameterInfo() { Value = p.Guardian2HomePhone, Nullable = true };
                pi[32] = new ParameterInfo() { Value = p.Guardian2WorkPhone, Nullable = true };
                pi[33] = new ParameterInfo() { Value = p.Guardian2Notes, Nullable = true };
                pi[34] = new ParameterInfo() { Value = p.Guardian3FirstName, Nullable = true };
                pi[35] = new ParameterInfo() { Value = p.Guardian3LastName, Nullable = true };
                if (p.Guardian3RelationshipObject == null)
                {
                    pi[36] = new ParameterInfo() { Value = null, Nullable = true };
                }
                else
                {
                    pi[36] = new ParameterInfo() { Value = p.Guardian3RelationshipObject.ID, Nullable = true };
                }
                pi[37] = new ParameterInfo() { Value = p.Guardian3Email, Nullable = true };
                pi[38] = new ParameterInfo() { Value = p.Guardian3CellPhone, Nullable = true };
                pi[39] = new ParameterInfo() { Value = p.Guardian3HomePhone, Nullable = true };
                pi[40] = new ParameterInfo() { Value = p.Guardian3WorkPhone, Nullable = true };
                pi[41] = new ParameterInfo() { Value = p.Guardian3Notes, Nullable = true };
                pi[42] = new ParameterInfo() { Value = p.Notes, Nullable = true };
                if (p.InsuranceID.HasValue)
                {
                    pi[43] = new ParameterInfo { Value = p.InsuranceID.Value, Nullable = true };
                }
                else
                {
                    pi[43] = new ParameterInfo { Value = null, Nullable = true };
                }
                pi[44] = new ParameterInfo { Value = p.PhysicianName, Nullable = true };
                pi[45] = new ParameterInfo { Value = p.PhysicianAddress, Nullable = true };
                pi[46] = new ParameterInfo { Value = p.PhysicianPhone, Nullable = true };
                pi[47] = new ParameterInfo { Value = p.PhysicianFax, Nullable = true };
                pi[48] = new ParameterInfo { Value = p.PhysicianEmail, Nullable = true };
                pi[49] = new ParameterInfo { Value = p.PhysicianContact, Nullable = true };
                pi[50] = new ParameterInfo { Value = p.PhysicianNotes, Nullable = true };
                pi[51] = new ParameterInfo { Value = p.HighRisk, Nullable = true };
                cmd.AddParameters(pi);
                try
                {
                    p.ID = cmd.InsertToIdentity();
                    PatientSearchService.UpdateEntry(p.ID.Value);
                    try
                    {
                        CreateNewPatientCase(p.ID.Value);
                    }
                    catch (Exception e)
                    {
                        Exceptions.Handle(e);
                        DeletePatient(p.ID.Value);
                        PatientSearchService.Remove(p.ID.Value);
                        throw e;
                    }
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        void CreateNewPatientCase(int patientID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                var statusReason = Domain.Cases.Case.DefaultStatusReason;
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.Cases (PatientID, CaseStatus, CaseStatusReason, CaseStartDate) VALUES (@PatientID, 0, @StatusReason, GETDATE());";
                cmd.Parameters.AddWithValue("@PatientID", patientID);
                cmd.Parameters.AddWithValue("@StatusReason", statusReason);
                try
                {
                    cmd.InsertToIdentity();
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw e;
                }
            }
        }


        // GENERIC/SUPPORTING STUFF
        private readonly string connectionString;
        private readonly PatientSearchService PatientSearchService;
        private readonly CoreContext Context;


        public PatientRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            Context = AppService.Current.DataContextV2;
            PatientSearchService = new PatientSearchService(Context);
        }


    }
}