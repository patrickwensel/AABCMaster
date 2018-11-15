using AABC.Data.V2;
using AABC.Domain2.Cases;
using AABC.Web.App.Staffing.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.App.Staffing
{

    public class StaffingRepository
    {

        public StaffingLog GetByID(int id)
        {
            return Context.StaffingLog.SingleOrDefault(m => m.ID == id);
        }


        public void SaveStaffingLog(StaffingLogSaveRequest req)
        {
            var staffingLogDb = Context.StaffingLog.SingleOrDefault(m => m.ID == req.ID);
            if (staffingLogDb == null)
            {
                throw new Exception($"Staffing log with id '${req.ID}' does not exist.");
            }

            staffingLogDb.ParentalRestaffRequest = req.ParentalRestaffRequest;
            staffingLogDb.HoursOfABATherapy = req.HoursOfABATherapy;
            staffingLogDb.AidesRespondingNo = req.AidesRespondingNo;
            staffingLogDb.AidesRespondingMaybe = req.AidesRespondingMaybe;
            staffingLogDb.ScheduleRequest = req.ScheduleRequest;
            staffingLogDb.ProviderGenderPreference = req.ProviderGenderPreference?.ToString();
            staffingLogDb.SpecialAttentionNeeds.Clear();

            if (req.SpecialAttentionNeedIds != null && req.SpecialAttentionNeedIds.Length > 0)
            {
                var needsDb = Context.SpecialAttentionNeeds.Where(n => req.SpecialAttentionNeedIds.Contains(n.ID)).ToList();
                foreach (var attentionNeed in needsDb)
                {
                    staffingLogDb.SpecialAttentionNeeds.Add(attentionNeed);
                }
            }
            Context.SaveChanges();
        }

        public IEnumerable<ProviderListItemVM> GetSelectableProviders(int staffingLogID)
        {
            return Context.Database.SqlQuery<ProviderListItemVM>("[dbo].[GetSelectableProvidersForStaffingLog] @staffingLogID",
                    new SqlParameter("staffingLogID", staffingLogID)
                ).ToList();
        }

        public IEnumerable<SelectedProviderListItemVM> GetSelectedProviders(int staffingLogID)
        {
            return Context.Database.SqlQuery<SelectedProviderListItemVM>("[dbo].[GetSelectedProvidersByStaffingLog] @staffingLogID",
                    new SqlParameter("staffingLogID", staffingLogID)
                ).ToList();
        }

        public Domain2.Providers.Provider GetProvider(int providerId)
        {
            return Context.Providers.SingleOrDefault(m => m.ID == providerId);
        }

        public StaffingLogProvider GetStaffingLogProviderById(int staffingLogProviderID)
        {
            return Context.StaffingLogProviders.SingleOrDefault(m => m.ID == staffingLogProviderID);
        }

        public void SaveStaffingLogProvider(StaffingLogProvider e)
        {
            //Context.StaffingLogProviders.Attach(e);
            Context.SaveChanges();
        }

        public void AddProviders(int staffingLogId, IEnumerable<int> providers)
        {
            var entities = new List<StaffingLogProvider>();
            foreach (var i in providers)
            {
                entities.Add(new StaffingLogProvider
                {
                    StaffingLogID = staffingLogId,
                    ProviderID = i
                });
            }
            Context.StaffingLogProviders.AddRange(entities);
            Context.SaveChanges();
        }

        public void RemoveProviders(int staffingLogId, IEnumerable<int> providers)
        {
            var entities = Context.StaffingLogProviders.Where(m => providers.Contains(m.ID)).ToList();
            Context.StaffingLogProviders.RemoveRange(entities);
            Context.SaveChanges();
        }

        private readonly CoreContext Context;

        public StaffingRepository()
        {
            Context = AppService.Current.DataContextV2;
        }


    }
}