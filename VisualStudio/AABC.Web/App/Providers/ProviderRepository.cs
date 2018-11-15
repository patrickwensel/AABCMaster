using AABC.Data.V2;
using AABC.Domain.Cases;
using AABC.Domain2.Providers;
using AABC.DomainServices.Providers;
using AABC.DomainServices.Staffing;
using AABC.Web.App.Providers;
using AABC.Web.App.Providers.Models;
using AABC.Web.Models.Cases;
using AABC.Web.Models.Providers;
using Dymeng.Framework;
using Dymeng.Framework.Data;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.Repositories
{
    public enum ProviderFilter
    {
        Inactive = 0,
        Active = 1,
        Potential = 2,
        All = 3,
        [Description("All Except Inactive")]
        AllExceptInactive = 4,
    }

    public interface IProviderRepository
    {
        // PROVIDERS
        ProviderVM GetProvider(int id);
        ProviderVM GetProvider();
        IEnumerable<ProvidersListItemVM> GetProviderListItems(ProviderFilter status);
        bool DeleteProvider(int id);
        void SaveProvider(ProviderVM model);

        // PROVIDER TYPES
        int GetProviderTypeAssociationCount(int id);
        IEnumerable<ProviderTypesListItemVM> GetProviderTypesListItems();
        void DeleteProviderType(int id);
        void SaveProviderType(ProviderTypeVM providerType);
        IEnumerable<CaseVM> GetProviderCaseDictionaryItems(int providerID);
        IEnumerable<CaseHoursListItemVM> GetProviderHours(int providerID, DateTime selectedDate);

        //Insurance Credentials
        //IEnumerable<ProviderInsuranceCredential> GetInsuranceCredentials(int providerID);
        //ProviderInsuranceCredential GetInsuranceCredential(int insuranceCredentialId);
        //void SaveInsuranceCredentials(ProviderInsuranceCredential insuranceCredential);
        //void DeleteInsuranceCredential(int insuranceCredentialId);
        void AddProviderRate(int providerId, DateTime effectiveDate, int rate);
        IEnumerable<ProviderSubTypesListItemVM> GetSubTypes(int providerID);
    }


    public class ProviderRepository : IProviderRepository
    {
        private readonly string connectionString;
        private readonly CoreContext Context;
        private readonly ProviderSearchService ProviderSearchService;
        private readonly StaffingService StaffingService;

        public ProviderRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            Context = AppService.Current.DataContextV2;
            ProviderSearchService = new ProviderSearchService(Context);
            StaffingService = new StaffingService(Context);
        }


        public IEnumerable<CaseHoursListItemVM> GetProviderHours(int providerID, DateTime selectedDate)
        {
            var hours = new Data.Services.CaseService()
                    .GetCaseHoursByProvider(providerID, selectedDate)
                    .Where(
                        x => (
                            x.Status == AuthorizationHoursStatus.FinalizedByProvider
                            || x.Status == AuthorizationHoursStatus.ScrubbedByAdmin
                            || x.Status == AuthorizationHoursStatus.ProcessedComplete
                        )
                        && (x.Date >= selectedDate)
                        && (x.Date < selectedDate.AddMonths(1))
                    )
                    .OrderBy(x => x.Case.Patient.FirstName)
                    .ThenBy(x => x.Case.Patient.LastName)
                    .ThenBy(x => x.Date)
                    .ThenBy(x => x.TimeIn)
                    .ToList();
            var items = hours.Select(hour => new CaseHoursListItemVM
            {
                AuthCode = hour.Authorization?.Code,
                Billable = hour.BillableHours,
                CaseAuthID = hour.Authorization?.ID,
                Date = hour.Date,
                Hours = hour.HoursTotal,
                HoursID = hour.ID.Value,
                Notes = hour.Notes,
                Payable = hour.PayableHours,
                PatientName = hour.Case.Patient.CommonName,
                StatusCode = GetTimeBillStatusCodeDisplay(hour.Status),
                StatusID = hour.Status,
                Service = hour.Service,
                TimeIn = hour.TimeIn,
                TimeOut = hour.TimeOut,
                Paid = hour.PayableRef == null ? false : true,
                Billed = hour.BillingRef == null ? false : true,
                HasCatalystData = hour.HasCatalystData,
                CaseAuth = hour.Authorization != null ? new TimeBillGridAuthItemVM
                {
                    ID = hour.Authorization.CaseAuthorizationID,
                    Code = hour.Authorization.Code
                } : null
            });
            return items;
        }


        public IEnumerable<CaseVM> GetProviderCaseDictionaryItems(int providerID)
        {
            var cases = from cp in Context.CaseProviders
                        join c in Context.Cases on cp.CaseID equals c.ID
                        join p in Context.Patients on c.PatientID equals p.ID
                        where cp.ProviderID == providerID
                        orderby p.FirstName, p.LastName
                        select new CaseVM
                        {
                            Active = cp.Active,
                            CaseID = cp.CaseID,
                            PatientName = p.FirstName + " " + p.LastName,
                            Status = c.Status
                        };
            return cases.ToList();
        }


        public ProviderVM GetProvider(int id)
        {
            var provider = Context.Providers.Find(id);
            if (provider == null)
            {
                return null;
            }
            var model = new ProviderVM
            {
                ID = provider.ID,
                Active = provider.Status == ProviderStatus.Active,
                Status = provider.Status,
                IsHired = provider.IsHired,
                FirstName = provider.FirstName,
                LastName = provider.LastName,
                CompanyName = provider.CompanyName,
                Phone = provider.Phone,
                Phone2 = provider.Phone2,
                Fax = provider.Fax,
                Email = provider.Email,
                Address1 = provider.Address1,
                Address2 = provider.Address2,
                City = provider.City,
                State = provider.State,
                Zip = provider.Zip,
                Notes = provider.Notes,
                Availability = provider.Availability,
                HourlyRate = provider.HourlyRate,
                HasBackgroundCheck = provider.HasBackgroundCheck,
                HasReferences = provider.HasReferences,
                HasResume = provider.HasResume,
                CanCall = provider.CanCall,
                CanReachByPhone = provider.CanReachByPhone,
                CanEmail = provider.CanEmail,
                DocumentStatus = provider.DocumentStatus,
                LBA = provider.LBA,
                NPI = provider.NPI,
                CertificationID = provider.CertificationID,
                CertificationState = provider.CertificationState,
                CertificationRenewalDate = provider.CertificationRenewalDate,
                W9Date = provider.W9Date,
                CAQH = provider.CAQH,
                ResumeFileName = provider.ResumeFileName,
                ResumeLocation = provider.ResumeLocation,
                PayrollID = provider.PayrollID,
                Gender = provider.ProviderGender?[0],
                HireDate = provider.HireDate,
                Type = new ProviderTypeVM
                {
                    ID = provider.ProviderType.ID,
                    DateCreated = provider.ProviderType.DateCreated,
                    Code = provider.ProviderType.Code,
                    Name = provider.ProviderType.Name,
                    IsOutsourced = provider.ProviderType.IsOutsourced,
                    CanSuperviseCase = provider.ProviderType.CanSupervise
                },
                ProviderSubTypeID = provider.ProviderSubTypeID,
                ServiceZipCodes = provider.ServiceZipCodes.ToList()
            };
            // TODO: refactor to use Domain2 entities.
            return model;
        }


        public ProviderVM GetProvider()
        {
            return new ProviderVM();
        }


        public IEnumerable<ProvidersListItemVM> GetProviderListItems(ProviderFilter filter)
        {
            var results = ProviderSearchService.GetAll();
            switch (filter)
            {
                case ProviderFilter.All:
                    break;
                case ProviderFilter.AllExceptInactive:
                    results = results.Where(x => x.Status != ProviderStatus.Inactive);
                    break;
                default:
                    var providerStatus = MapProviderFilter(filter);
                    results = results.Where(x => x.Status == providerStatus);
                    break;
            }
            return results.OrderBy(p => p.LastName);
        }


        // Return true if successfully deleted, false otherwise
        // (could fail due to validation if there's certain history attached to the provider)
        // throws exception on unexpected failure
        public bool DeleteProvider(int id)
        {
            try
            {
                var removalService = new RemovalService(Context);
                var ppUserDbConnectionString = ConfigurationManager.ConnectionStrings["ProviderPortalConnection"].ConnectionString;
                if (removalService.DeleteProvider(id, ppUserDbConnectionString))
                {
                    ProviderSearchService.Remove(id);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Exceptions.Handle(e);
                throw new DataException(e.Message, e);
            }
        }


        public void SaveProvider(ProviderVM model)
        {
            var provider = Context.Providers
                               .Include("ServiceZipCodes")
                               .FirstOrDefault(p => p.ID == model.ID)
                           ?? new Domain2.Providers.Provider
                           {
                               ServiceZipCodes = new List<ProviderServiceZipCode>()
                           };
            provider.FirstName = model.FirstName;
            provider.LastName = model.LastName;
            provider.IsHired = model.IsHired;
            provider.ProviderTypeID = model.Type.ID ?? 0;
            provider.ProviderSubTypeID = model.ProviderSubTypeID > 0 ? model.ProviderSubTypeID : null;
            provider.CompanyName = model.CompanyName;
            provider.Phone = model.Phone;
            provider.Email = model.Email;
            provider.Address1 = model.Address1;
            provider.Address2 = model.Address2;
            provider.City = model.City;
            provider.State = model.State;
            provider.Zip = model.Zip;
            provider.NPI = model.NPI;
            provider.HourlyRate = model.HourlyRate;
            provider.Phone2 = model.Phone2;
            provider.Fax = model.Fax;
            provider.Notes = model.Notes;
            provider.Availability = model.Availability;
            provider.HasBackgroundCheck = model.HasBackgroundCheck;
            provider.HasReferences = model.HasReferences;
            provider.HasResume = model.HasResume;
            provider.CanCall = model.CanCall;
            provider.CanReachByPhone = model.CanReachByPhone;
            provider.CanEmail = model.CanEmail;
            provider.DocumentStatus = model.DocumentStatus;
            provider.LBA = model.LBA;
            provider.CertificationID = model.CertificationID;
            provider.CertificationState = model.CertificationState;
            provider.CertificationRenewalDate = model.CertificationRenewalDate;
            provider.W9Date = model.W9Date;
            provider.CAQH = model.CAQH;
            provider.PayrollID = model.PayrollID;
            provider.ProviderGender = model.Gender.ToString();
            provider.HireDate = model.HireDate;
            provider.Status = model.Status;
            UpdateProviderZipCodes(model, provider);
            if (provider.ID == 0)
            {
                if (model.HourlyRate.HasValue)
                {
                    var rate = new ProviderRate
                    {
                        Rate = model.HourlyRate.Value,
                        Type = RateType.Hourly
                    };
                    provider.ProviderRates.Add(rate);
                }
                Context.Providers.Add(provider);
            }
            SaveResumeFile(provider, model);
            Context.SaveChanges();
            ProviderSearchService.UpdateEntry(provider.ID);
            StaffingService.PerformCheckByProviderId(provider.ID);
        }


        #region ProviderType
        public int GetProviderTypeAssociationCount(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT COUNT(*) AS AssocCount FROM dbo.Providers WHERE ProviderType = @TypeID";
                cmd.Parameters.AddWithValue("@TypeID", id);
                try
                {
                    DataRow row = cmd.GetRow();
                    return row.ToInt("AssocCount");
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        public void DeleteProviderType(int id)
        {
            if (GetProviderTypeAssociationCount(id) != 0)
            {
                throw new DataConstraintException("Provider Type has associated Providers.  Unable to delete.");
            }
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.ProviderTypes WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", id);
                try
                {
                    cmd.ExecuteNonQueryToInt();
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        public IEnumerable<ProviderTypesListItemVM> GetProviderTypesListItems()
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT " +
                    "  t.ID, t.DateCreated, t.ProviderTypeCode, t.ProviderTypeName, " +
                    "  t.ProviderTypeIsOutsourced, t.ProviderTypeCanSuperviseCase, " +
                    "  COUNT(a.ID) AS AssociationsCount " +
                    "FROM dbo.ProviderTypes AS t " +
                    "LEFT JOIN dbo.Providers AS a ON a.ProviderType = t.ID " +
                    "GROUP BY t.ID, t.DateCreated, t.ProviderTypeCode, t.ProviderTypeName, " +
                    "  t.ProviderTypeIsOutsourced, t.ProviderTypeCanSuperviseCase;";
                try
                {
                    var table = cmd.GetTable();
                    var types = new List<ProviderTypesListItemVM>();
                    foreach (DataRow r in table.Rows)
                    {

                        var type = new ProviderTypesListItemVM
                        {
                            ID = r.ToInt("ID"),
                            DateCreated = r.ToDateTime("DateCreated"),
                            Code = r.ToStringValue("ProviderTypeCode"),
                            Name = r.ToStringValue("ProviderTypeName"),
                            IsOutsourced = r.ToBool("ProviderTypeIsOutsourced"),
                            CanSuperviseCase = r.ToBool("ProviderTypeCanSuperviseCase"),
                            AssociatedProvidersCount = r.ToInt("AssociationsCount")
                        };

                        types.Add(type);
                    }
                    return types;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        public void SaveProviderType(ProviderTypeVM providerType)
        {
            if (providerType.ID.HasValue)
            {
                UpdateExistingProviderType(providerType);
            }
            else
            {
                SaveNewProviderType(providerType);
            }
        }
        #endregion


        #region InsuranceCredentials
        //public IEnumerable<ProviderInsuranceCredential> GetInsuranceCredentials(int providerID)
        //{
        //    using (var conn = new SqlConnection(connectionString))
        //    using (var cmd = new SqlCommand())
        //    {
        //        cmd.Connection = conn;
        //        cmd.CommandText = "select c.*, i.InsuranceName from ProviderInsuranceCredentials c inner join Insurances i on c.InsuranceId = i.Id WHERE ProviderID = @ProviderID;";
        //        cmd.Parameters.AddWithValue("@ProviderID", providerID);
        //        try
        //        {
        //            var table = cmd.GetTable();
        //            var credentials = new List<ProviderInsuranceCredential>();
        //            foreach (DataRow r in table.Rows)
        //            {

        //                var a = new ProviderInsuranceCredential
        //                {
        //                    Id = r.ToInt("Id"),
        //                    ProviderId = r.ToInt("ProviderId"),
        //                    InsuranceId = r.ToInt("InsuranceId"),
        //                    InsuranceName = r.ToStringValue("InsuranceName"),
        //                    StartDate = r.ToDateTimeOrNull("StartDate"),
        //                    EndDate = r.ToDateTimeOrNull("EndDate")
        //                };

        //                credentials.Add(a);
        //            }
        //            return credentials;
        //        }
        //        catch (Exception e)
        //        {
        //            Exceptions.Handle(e);
        //            throw new DataException(e.Message, e);
        //        }
        //    }
        //}


        //public ProviderInsuranceCredential GetInsuranceCredential(int insuranceCredentialId)
        //{
        //    using (var conn = new SqlConnection(this.connectionString))
        //    using (var cmd = new SqlCommand())
        //    {
        //        cmd.Connection = conn;
        //        cmd.CommandText = "select c.*, i.InsuranceName from ProviderInsuranceCredentials c inner join Insurances i on c.InsuranceId = i.Id WHERE c.Id = @Id;";
        //        cmd.Parameters.AddWithValue("@Id", insuranceCredentialId);
        //        try
        //        {
        //            var table = cmd.GetTable();
        //            var credentials = new List<Domain.Providers.ProviderInsuranceCredential>();
        //            DataRow r = table.Rows[0];
        //            var a = new ProviderInsuranceCredential
        //            {
        //                Id = r.ToInt("Id"),
        //                ProviderId = r.ToInt("ProviderId"),
        //                InsuranceId = r.ToInt("InsuranceId"),
        //                InsuranceName = r.ToStringValue("InsuranceName"),
        //                StartDate = r.ToDateTimeOrNull("StartDate"),
        //                EndDate = r.ToDateTimeOrNull("EndDate")
        //            };
        //            return a;
        //        }
        //        catch (Exception e)
        //        {
        //            Exceptions.Handle(e);
        //            throw new DataException(e.Message, e);
        //        }
        //    }
        //}


        //public void SaveInsuranceCredentials(ProviderInsuranceCredential insuranceCredential)
        //{
        //    var sql = "";
        //    using (var conn = new SqlConnection(connectionString))
        //    using (var cmd = new SqlCommand())
        //    {
        //        conn.Open();
        //        cmd.Connection = conn;
        //        if (insuranceCredential.Id == 0)
        //        {
        //            sql = "INSERT INTO [ProviderInsuranceCredentials] ([ProviderId],[InsuranceId],[StartDate],[EndDate]) " +
        //                  "VALUES(@ProviderId, @InsuranceId, @StartDate, @EndDate)";
        //        }
        //        else
        //        {
        //            sql = "UPDATE [ProviderInsuranceCredentials] SET [ProviderId] = @ProviderId, [InsuranceId] = @InsuranceId, " +
        //                    "[StartDate] = @StartDate, [EndDate] = @EndDate WHERE Id = @Id";
        //            cmd.Parameters.AddWithValue("@Id", insuranceCredential.Id);
        //        }
        //        cmd.CommandText = sql;
        //        cmd.Parameters.AddWithValue("@ProviderId", insuranceCredential.ProviderId);
        //        cmd.Parameters.AddWithValue("@InsuranceId", insuranceCredential.InsuranceId);
        //        cmd.Parameters.AddWithNullableValue("@StartDate", insuranceCredential.StartDate);
        //        cmd.Parameters.AddWithNullableValue("@EndDate", insuranceCredential.EndDate);
        //        cmd.ExecuteNonQuery();
        //        ProviderSearchService.UpdateEntry(insuranceCredential.ProviderId);
        //    }
        //}


        //public void DeleteInsuranceCredential(int insuranceCredentialId)
        //{
        //    string sql = "";
        //    using (SqlConnection conn = new SqlConnection(this.connectionString))
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        conn.Open();
        //        cmd.Connection = conn;
        //        sql = "delete from [ProviderInsuranceCredentials] WHERE Id = @Id";
        //        cmd.CommandText = sql;
        //        cmd.Parameters.AddWithValue("@Id", insuranceCredentialId);
        //        cmd.ExecuteNonQuery();
        //    }
        //}
        #endregion


        public void AddProviderRate(int providerId, DateTime effectiveDate, int rate)
        {
            Context.ProviderRates.Add(new ProviderRate
            {
                EffectiveDate = effectiveDate,
                Rate = rate,
                Type = RateType.Hourly,
                ProviderID = providerId
            });
            Context.SaveChanges();
        }


        public IEnumerable<ProviderSubTypesListItemVM> GetSubTypes(int providerID)
        {
            var elements = Context.ProviderSubTypes.Where(m => m.ParentTypeID == providerID)
                        .OrderBy(m => m.Code)
                        .ToList()
                        .Select(m => new ProviderSubTypesListItemVM { ID = m.ID, Code = m.Code, Name = m.Name })
                        .ToList();
            if (elements.Count > 0)
            {
                elements.Insert(0, new ProviderSubTypesListItemVM { ID = 0, Code = "NONE" });
            }
            return elements;
        }


        private void UpdateExistingProviderType(ProviderTypeVM model)
        {
            using (var conn = new SqlConnection(this.connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "UPDATE dbo.ProviderTypes SET " +
                    "    ProviderTypeCode = @Code, ProviderTypeName = @Name, " +
                    "    ProviderTypeIsOutsourced = @Outsourced, ProviderTypeCanSuperviseCase = @CanSupervise " +
                    "WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", model.ID.Value);
                cmd.Parameters.AddWithValue("@Code", model.Code);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Outsourced", model.IsOutsourced);
                cmd.Parameters.AddWithValue("@CanSupervise", model.CanSuperviseCase);
                try
                {
                    cmd.ExecuteNonQueryToInt();
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        private void SaveNewProviderType(ProviderTypeVM model)
        {
            using (var conn = new SqlConnection(this.connectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "INSERT INTO dbo.ProviderTypes (ProviderTypeCode, ProviderTypeName, ProviderTypeIsOutsourced, ProviderTypeCanSuperviseCase) VALUES (" +
                    "  @Code, @Name, @IsOutsourced, @CanSupervise);";
                cmd.Parameters.AddWithValue("@Code", model.Code);
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@IsOutsourced", model.IsOutsourced);
                cmd.Parameters.AddWithValue("@CanSupervise", model.CanSuperviseCase);
                try
                {
                    int? i = cmd.InsertToIdentity();
                    if (i.HasValue)
                    {
                        model.ID = i.Value;
                    }
                    return;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }


        private void UpdateProviderZipCodes(ProviderVM model, Domain2.Providers.Provider provider)
        {
            if (!string.IsNullOrEmpty(model.ServiceZips))
            {
                var zips = model.ServiceZips
                    .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(z => z.Trim())
                    .ToList();
                var codesToDelete = Context.ProviderServiceZipCodes
                    .Where(z => z.ProviderID == model.ID && !zips.Contains(z.ZipCode))
                    .ToList();
                codesToDelete.ForEach(c => Context.ProviderServiceZipCodes.Remove(c));
                var codesToAdd = zips
                    .Where(z => provider.ServiceZipCodes.All(szc => szc.ZipCode != z))
                    .ToList();
                foreach (var code in codesToAdd)
                {
                    provider.ServiceZipCodes.Add(new ProviderServiceZipCode
                    {
                        IsPrimary = true,
                        ZipCode = code
                    });
                }
            }
            else
            {
                provider.ServiceZipCodes.Clear();
            }
        }


        private void SaveResumeFile(Domain2.Providers.Provider provider, ProviderVM model)
        {
            var resumeFilesCount = model.ResumeFile.Count();
            if (resumeFilesCount == 0 || (resumeFilesCount == 1 && model.ResumeFile.First().FileName == ""))
            {
                return;
            }
            var oldLocation = provider.ResumeLocation;
            if (!string.IsNullOrEmpty(oldLocation))
            {
                System.IO.File.Delete(AppService.Current.Settings.UploadDirectory + oldLocation);
            }
            var uploadedFile = model.ResumeFile.First();
            var saveName = model.ID + uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf('.'));
            var partialPath = "\\provider_resumes\\" + model.LastName.Substring(0, 1).ToUpper();
            var saveDir = AppService.Current.Settings.UploadDirectory + partialPath;
            var fullPath = saveDir + "\\" + saveName;
            System.IO.Directory.CreateDirectory(saveDir);
            using (var fstream = new System.IO.FileStream(fullPath, System.IO.FileMode.Create))
            {
                fstream.Write(uploadedFile.FileBytes, 0, uploadedFile.FileBytes.Length);
            }
            provider.ResumeFileName = uploadedFile.FileName;
            provider.ResumeLocation = partialPath + "\\" + saveName;
        }


        private string GetTimeBillStatusCodeDisplay(AuthorizationHoursStatus status)
        {
            switch (status)
            {
                case AuthorizationHoursStatus.FinalizedByProvider:
                    return "E";
                case AuthorizationHoursStatus.ScrubbedByAdmin:
                    return "F";
                default:
                    return "F";
            }
        }


        private static ProviderStatus MapProviderFilter(ProviderFilter filter)
        {
            switch (filter)
            {
                case ProviderFilter.Active:
                    return ProviderStatus.Active;
                case ProviderFilter.Inactive:
                    return ProviderStatus.Inactive;
                case ProviderFilter.Potential:
                    return ProviderStatus.Potential;
                default:
                    throw new InvalidProgramException();
            }
        }
    }
}
