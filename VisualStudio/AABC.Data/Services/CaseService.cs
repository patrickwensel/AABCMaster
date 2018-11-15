using AABC.Domain.Cases;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Data.Services
{


    public interface ICaseService
    {
        List<Case> GetActiveCasesByProvider(int providerID, int visibleAfterEndDateDays);
        int? GetAssociatedCaseAuthID(int providerTypeID, int serviceID);
        AuthorizationClass GetAuthClassByCode(string authClassCode);
        Case GetCase(int caseID);
        List<CaseAuthorizationHours> GetCaseAuthorizationHoursProviderAndCase(int providerID, int caseID);
        List<CaseAuthorization> GetCaseAuthorizationsAndGeneralHours(int caseID, bool omitBCBAAuths = false);
        List<CaseAuthorization> GetCaseAuthorizationsAndHours(int caseID);
        List<CaseAuthorizationHours> GetCaseHoursByCase(int caseID);
        List<CaseAuthorizationHours> GetCaseHoursByCase(int caseID, DateTime? cutoff = null);
        List<CaseAuthorizationHours> GetCaseHoursByCaseAndDate(int caseID, DateTime date);
        List<CaseAuthorizationHours> GetCaseHoursByCaseByProvider(int caseID, int providerID, DateTime? cutoffDate = null);
        List<CaseAuthorizationHours> GetCaseHoursByDateRange(DateTime startDate, DateTime endDate);
        CaseAuthorizationHours GetCaseHoursItem(int hoursID);
        List<MonthlyCasePeriod> GetCaseMonthlyPeriods(int caseID, DateTime startDate, DateTime endDate);
        CaseProvider GetCaseProviderByProviderAndCaseIDs(int providerID, int caseID);
        List<Case> GetCasesByPatientName(string firstName, string lastName);
        List<TimeScrubOverviewItem> GetCaseTimeScrubOverviewItems(DateTime startDate, DateTime endDate);
        List<Domain.General.GuardianRelationship> GetGuardianRelationships();
        List<Service> GetServices();
        List<Service> GetServicesByProviderType(int providerTypeID);
    }



    public class CaseService : ICaseService
    {


        public int? GetAssociatedCaseAuthID(int providerTypeID, int serviceID)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT AssociatedAuthClassID FROM dbo.ProviderTypeServices WHERE ProviderTypeID = @TypeID AND ServiceID = @ServiceID;";

                cmd.Parameters.AddWithValue("@TypeID", providerTypeID);
                cmd.Parameters.AddWithValue("@ServiceID", serviceID);

                DataRow r = cmd.GetRowOrNull();
                if (r == null)
                {
                    return null;
                }
                else
                {
                    return r.ToIntOrNull("AssociatedAuthClassID");
                }
            }
        }

        public Case GetCase(int caseID)
        {

            var context = new Models.CoreEntityModel();

            var c = context.Cases.Find(caseID);
            return Mappings.CaseMappings.Case(c);
        }

        public List<Case> GetCasesByPatientName(string firstName, string lastName)
        {

            var context = new Models.CoreEntityModel();

            var q = from cases in context.Cases
                    join patients in context.Patients on cases.PatientID equals patients.ID
                    where patients.PatientFirstName == firstName && patients.PatientLastName == lastName
                    select cases;

            var entities = q.ToList();

            return Mappings.CaseMappings.Cases(q, true).ToList();
        }

        public List<TimeScrubOverviewItem> GetCaseTimeScrubOverviewItems(DateTime startDate, DateTime endDate)
        {

            var context = new Models.CoreEntityModel();

            var items = new List<TimeScrubOverviewItem>();

            var data = context.Database.SqlQuery<Models.Sprocs.CaseTimeScrubOverview>(
                "GetCaseTimeScrubOverview @StartDate, @EndDate",
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)
                ).ToList();

            foreach (var dataItem in data)
            {
                var item = new TimeScrubOverviewItem();
                item.ID = dataItem.CaseID;
                item.FirstName = dataItem.PatientFirstName;
                item.LastName = dataItem.PatientLastName;
                item.ActiveProviders = dataItem.CountOfActiveProviders;
                item.ProvidersWithHours = dataItem.CountOfProvidersWithHours;
                item.ProvidersFinalized = dataItem.CountOfProvidersFinalized;
                item.ScrubbedRecords = dataItem.CountOfScrubbedRecords;
                item.UnscrubbedRecords = dataItem.CountOfUnscrubbedRecords;
                item.CommitedRecords = dataItem.CountOfCommittedRecords;
                item.BilledRecords = dataItem.CountOfBilledRecords;
                item.PaidRecords = dataItem.CountOfPaidRecords;
                item.TotalPayable = dataItem.TotalPayable;
                item.TotalBillable = dataItem.TotalBillable;
                item.BCBABillable = dataItem.BCBABillable;
                item.AideBillable = dataItem.AideBillable;
                item.BCBAPercent = dataItem.BCBAPercent;

                items.Add(item);
            }

            return items;
        }

        public List<MonthlyCasePeriod> GetCaseMonthlyPeriods(int caseID, DateTime startDate, DateTime endDate)
        {

            var context = new Models.CoreEntityModel();

            try
            {

                var items = new List<MonthlyCasePeriod>();

                var data = context.Database.SqlQuery<CaseMonthlyPeriodSprocDTO>(
                    "GetCaseMonthlyPeriods @CaseID, @StartDate, @EndDate",
                    new SqlParameter("@CaseID", caseID),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate)
                    ).ToList();

                foreach (var dataItem in data)
                {
                    var item = new Domain.Cases.MonthlyCasePeriod();
                    item.ID = dataItem.ID;
                    item.FirstDayOfMonth = dataItem.PeriodFirstDayOfMonth;
                    items.Add(item);
                }
                return items;
            }
            catch (Exception e)
            {
                Dymeng.Framework.Exceptions.Handle(e);
                throw e;
            }
        }

        private class CaseMonthlyPeriodSprocDTO
        {
            public int? ID { get; set; }
            public DateTime PeriodFirstDayOfMonth { get; set; }
        }

        public CaseAuthorizationHours GetCaseHoursItem(int hoursID)
        {

            var item = new CaseAuthorizationHours();

            var context = new Models.CoreEntityModel();

            var entity = context.CaseAuthHours.Find(hoursID);

            if (entity == null)
            {
                return null;
            }
            else
            {
                return Mappings.AuthorizationMappings.CaseAuthorizationHour(entity);
            }

        }



        public List<CaseAuthorizationHours> GetCaseHoursByDateRange(DateTime startDate, DateTime endDate)
        {

            var list = new List<CaseAuthorizationHours>();

            var context = new Models.CoreEntityModel();

            var entities = context.CaseAuthHours.Where(x => x.HoursDate >= startDate && x.HoursDate <= endDate).ToList();

            list = Mappings.AuthorizationMappings.CaseAuthorizationHours(entities);

            return list;

        }



        public List<CaseAuthorizationHours> GetCaseHoursByCaseAndDate(int caseID, DateTime date)
        {

            List<CaseAuthorizationHours> list = new List<CaseAuthorizationHours>();

            var context = new Models.CoreEntityModel();

            var entities = context.CaseAuthHours.Where(x => x.CaseID == caseID && x.HoursDate == date).ToList();

            list = Mappings.AuthorizationMappings.CaseAuthorizationHours(entities);

            return list;

        }

        public List<CaseAuthorizationHours> GetCaseHoursByCase(int caseID)
        {
            return GetCaseHoursByCase(caseID, null);
        }

        public List<CaseAuthorizationHours> GetCaseHoursByCase(int caseID, DateTime? cutoff = null)
        {

            List<CaseAuthorizationHours> list = new List<CaseAuthorizationHours>();

            var context = new Models.CoreEntityModel();

            IQueryable<Models.CaseAuthHour> q;

            if (cutoff == null)
            {

                q = (from hours in context.CaseAuthHours
                     where hours.CaseID == caseID
                     select hours)
                     .Union(
                    from hours in context.CaseAuthHours
                    join auths in context.CaseAuthCodes on hours.CaseAuthID equals auths.ID
                    where auths.CaseID == caseID && hours.CaseAuthID == auths.ID
                    select hours
                );

            }
            else
            {

                q = (from hours in context.CaseAuthHours
                     where hours.CaseID == caseID && hours.HoursDate >= cutoff.Value
                     select hours)
                     .Union(
                    from hours in context.CaseAuthHours
                    join auths in context.CaseAuthCodes on hours.CaseAuthID equals auths.ID
                    where auths.CaseID == caseID && hours.CaseAuthID == auths.ID && hours.HoursDate >= cutoff.Value
                    select hours
                );

            }



            var entities = q.ToList();

            list = Mappings.AuthorizationMappings.CaseAuthorizationHours(entities);

            foreach (var hours in list)
            {
                hours.Provider = Mappings.CaseMappings.CaseProvider(context.CaseProviders.Where(x => x.ProviderID == hours.Provider.ID.Value).FirstOrDefault());
                hours.Authorization = Mappings.AuthorizationMappings.CaseAuthorization(context.CaseAuthCodes.Where(x => x.ID == hours.Authorization.ID).FirstOrDefault());
            }

            return list;

        }

        public List<CaseAuthorizationHours> GetCaseAuthorizationHoursProviderAndCase(int providerID, int caseID)
        {

            List<CaseAuthorizationHours> list = new List<CaseAuthorizationHours>();

            var context = new Models.CoreEntityModel();

            var q = (from hours in context.CaseAuthHours
                     where hours.CaseID == caseID && hours.CaseProviderID == providerID
                     select hours)
                     .Union(
                    from hours in context.CaseAuthHours
                    join auths in context.CaseAuthCodes on hours.CaseAuthID equals auths.ID
                    where auths.CaseID == caseID && hours.CaseAuthID == auths.ID && hours.CaseProviderID == providerID
                    select hours
                );

            var entities = q.ToList();

            list = Mappings.AuthorizationMappings.CaseAuthorizationHours(entities);

            return list;

        }

        public List<CaseAuthorization> GetCaseAuthorizationsAndGeneralHours(int caseID, bool omitBCBAAuths = false)
        {

            List<CaseAuthorization> list = new List<CaseAuthorization>();

            var context = new Models.CoreEntityModel();

            List<Models.CaseAuthCode> entities;

            if (omitBCBAAuths)
            {
                entities = context.CaseAuthCodes.Where(x => x.CaseID == caseID && x.CaseAuthClass.ID == 3).ToList();
            }
            else
            {
                entities = context.CaseAuthCodes.Where(x => x.CaseID == caseID).ToList();
            }

            foreach (var entity in entities)
            {
                var auth = Mappings.AuthorizationMappings.CaseAuthorization(entity);

                var generalHoursEntities = context.CaseAuthCodeGeneralHours.Where(x => x.CaseAuthID == auth.CaseAuthorizationID);
                auth.GeneralHours = Mappings.AuthorizationMappings.CaseAuthorizationGeneralHours(generalHoursEntities.ToList());

                list.Add(auth);
            }

            list = list.OrderBy(x => x.StartDate).ThenBy(x => x.AuthClass.Code).ToList();

            return list;
        }

        public CaseProvider GetCaseProviderByProviderAndCaseIDs(int providerID, int caseID)
        {

            var context = new Models.CoreEntityModel();

            var entity = context.CaseProviders.Where(x => x.ProviderID == providerID && x.CaseID == caseID).FirstOrDefault();
            entity.Provider = context.Providers.Where(x => x.ID == providerID).FirstOrDefault();
            if (entity == null)
            {
                return null;
            }
            else
            {
                return Mappings.CaseMappings.CaseProvider(entity);
            }

        }

        public List<CaseAuthorizationHours> GetCaseHoursByCaseByProvider(int caseID, int providerID, DateTime? cutoffDate = null)
        {

            var context = new Models.CoreEntityModel();

            IQueryable<Models.CaseAuthHour> q;

            if (cutoffDate == null)
            {

                q =
                (from auths in context.CaseAuthCodes
                 join hours in context.CaseAuthHours on auths.ID equals hours.CaseAuthID
                 where auths.CaseID == caseID && hours.CaseProviderID == providerID
                 select hours)
                .Union(
                    from hours in context.CaseAuthHours
                    where hours.CaseID == caseID && hours.CaseProviderID == providerID
                    select hours
                    );

            }
            else
            {

                q =
                (from auths in context.CaseAuthCodes
                 join hours in context.CaseAuthHours on auths.ID equals hours.CaseAuthID
                 where auths.CaseID == caseID && hours.CaseProviderID == providerID && hours.HoursDate >= cutoffDate.Value
                 select hours)
                .Union(
                    from hours in context.CaseAuthHours
                    where hours.CaseID == caseID && hours.CaseProviderID == providerID && hours.HoursDate >= cutoffDate.Value
                    select hours
                    );

            }



            var providerHours = q.ToList();

            var list = Mappings.AuthorizationMappings.CaseAuthorizationHours(providerHours);

            var services = GetServices();

            var providerService = new ProviderService();

            foreach (var hours in list)
            {
                hours.Service = services.Where(x => x.ID == hours.Service.ID).FirstOrDefault();
                var provider = providerService.GetProvider(hours.ProviderID);
                Domain.Cases.CaseProvider cp = new CaseProvider();
                cp.FirstName = provider.FirstName;
                cp.LastName = provider.LastName;
                hours.Provider = cp;

            }

            return list;

        }

        public List<Domain.General.GuardianRelationship> GetGuardianRelationships()
        {

            CacheServiceItems key = CacheServiceItems.GuardianRelationship;
            var list = CacheService.Get(key) as List<Domain.General.GuardianRelationship>;

            if (list != null)
            {
                return list;
            }
            else
            {

                list = new List<Domain.General.GuardianRelationship>();
                var context = new Models.CoreEntityModel();
                var entities = context.GuardianRelationships.ToList();
                list = Mappings.GeneralMappings.GuardianRelationships(entities).ToList();
                CacheService.Add(key, list);

                return list;
            }

        }

        public List<Service> GetServices()
        {

            CacheServiceItems key = CacheServiceItems.ProviderServices;
            var list = CacheService.Get(key) as List<Service>;

            if (list != null)
            {
                return list;
            }
            else
            {

                list = new List<Service>();

                var context = new Models.CoreEntityModel();

                var entities = context.Services.ToList();
                list = Mappings.CaseMappings.Services(entities);

                CacheService.Add(key, list);

                return list;
            }
        }

        public List<Service> GetServicesByProviderType(int providerTypeID)
        {

            // get from cache or db
            var cacheKey = "ServicesForProviderType" + providerTypeID;
            var list = CacheService.Get(cacheKey) as List<Service>;


            if (list != null)
            {
                return list;
            }
            else
            {

                list = new List<Service>();

                var context = new Models.CoreEntityModel();

                var entities = context.ProviderTypeServices.Where(x => x.ProviderTypeID == providerTypeID).ToList();
                foreach (var entity in entities)
                {
                    var serviceEntity = entity.Service;
                    list.Add(Mappings.CaseMappings.Service(serviceEntity));
                }

                CacheService.Add(cacheKey, list);
            }
            return list;
        }


        public List<Domain.Cases.Case> GetActiveCasesByProvider(int providerID, int visibleAfterEndDateDays)
        {

            var context = new Models.CoreEntityModel();

            DateTime cutoff = DateTime.Now.AddDays(visibleAfterEndDateDays * -1);

            var q = from c in context.Cases
                    join cp in context.CaseProviders on c.ID equals cp.CaseID
                    where cp.ProviderID == providerID
                        && cp.Active == true
                        && c.CaseStatus != (int)Domain.Cases.CaseStatus.History
                        && (!cp.ActiveStartDate.HasValue || cp.ActiveStartDate.Value <= DateTime.Now)
                        && (!cp.ActiveEndDate.HasValue || cp.ActiveEndDate.Value >= cutoff)
                    select c;

            var entities = q.ToList();

            return Mappings.CaseMappings.Cases(entities).ToList();

        }

        //public List<Domain.Cases.Case> GetActiveCasesByProvider(int providerID) {

        //    var context = new Models.CoreEntityModel();

        //    var q = from c in context.Cases
        //            join cp in context.CaseProviders on c.ID equals cp.CaseID
        //            where cp.ProviderID == providerID && cp.Active == true
        //            select c;

        //    var entities = q.ToList();

        //    return Mappings.CaseMappings.Cases(entities).ToList();

        //}

        public List<CaseAuthorization> GetCaseAuthorizationsAndHours(int caseID)
        {

            List<CaseAuthorization> list = new List<CaseAuthorization>();

            var context = new Models.CoreEntityModel();

            var entities = context.CaseAuthCodes.Where(x => x.CaseID == caseID).ToList();

            foreach (var entity in entities)
            {
                var auth = Mappings.AuthorizationMappings.CaseAuthorization(entity);

                var hoursEntities = context.CaseAuthHours.Where(x => x.CaseAuthID == auth.CaseAuthorizationID);
                auth.Hours = Mappings.AuthorizationMappings.CaseAuthorizationHours(hoursEntities.ToList());

                list.Add(auth);
            }

            return list;
        }

        public AuthorizationClass GetAuthClassByCode(string authClassCode)
        {

            var context = new Models.CoreEntityModel();

            var entity = context.CaseAuthClasses.Where(x => x.AuthClassCode == authClassCode).FirstOrDefault();
            if (entity == null)
            {
                return null;
            }
            else
            {
                return Mappings.AuthorizationMappings.AuthorizationClass(entity);
            }

        }

        public List<CaseAuthorizationHours> GetCaseHoursByProvider(int providerID, DateTime startDate)
        {

            DateTime endDate = startDate.AddMonths(1);

            List<CaseAuthorizationHours> list = new List<CaseAuthorizationHours>();

            var context = new Models.CoreEntityModel();

            IQueryable<Models.CaseAuthHour> q;

            q = from hours in context.CaseAuthHours
                where hours.CaseProviderID == providerID
                    && hours.HoursDate >= startDate && hours.HoursDate < endDate
                select hours
            ;

            var entities = q.ToList();

            // get any applicable cases
            List<int> caseIDs = entities.GroupBy(x => x.CaseID.Value).Select(x => x.First().CaseID.Value).ToList();
            var cases = new List<Case>();
            foreach (int caseID in caseIDs)
            {
                var c = Mappings.CaseMappings.Case(context.Cases.Find(caseID));
                cases.Add(c);
            }

            list = Mappings.AuthorizationMappings.CaseAuthorizationHours(entities);

            foreach (var hours in list)
            {
                hours.Provider = Mappings.CaseMappings.CaseProvider(context.CaseProviders.Where(x => x.ProviderID == hours.Provider.ID.Value).FirstOrDefault());
                hours.Authorization = Mappings.AuthorizationMappings.CaseAuthorization(context.CaseAuthCodes.Where(x => x.ID == hours.Authorization.ID).FirstOrDefault());
                hours.Case = cases.Where(x => x.ID == hours.CaseID).Single();
            }

            return list;




        }

        string connectionString;

        public CaseService()
        {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }

        public CaseService(string connectionString)
        {
            this.connectionString = connectionString;
        }

    }
}
