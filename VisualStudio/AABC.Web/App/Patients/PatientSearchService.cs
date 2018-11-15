using AABC.Cache;
using AABC.Data.V2;
using AABC.Web.App.Patients.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.App.Patients
{
    public class PatientSearchService
    {
        private readonly CoreContext Context;
        private readonly IHashCacheRepository<PatientsListItem2VM, int> CacheRepository;

        public PatientSearchService(CoreContext context)
        {
            int minutes = int.TryParse(ConfigurationManager.AppSettings["Cache.Patients.Expiration"] as string, out minutes) ? minutes : 60 * 24;
            Context = context;
            CacheRepository = new HashCacheRepository<PatientsListItem2VM, int>(AppService.Current.CacheClient, m => m.ID, minutes);
        }

        public IEnumerable<PatientsListItem2VM> GetAll()
        {
            IEnumerable<PatientsListItem2VM> results;
            if (!CacheRepository.IsLoaded())
            {
                results = GetPatients(null);
                CacheRepository.SetAll(results);
                return results;
            }
            return CacheRepository.GetAll();
        }

        public void UpdateEntry(int patientID)
        {
            UpdateEntries(new int[] { patientID });
        }

        public void UpdateEntries(IEnumerable<int> patientIDs)
        {
            if (CacheRepository.IsLoaded())
            {
                var elements = new List<PatientsListItem2VM>();
                foreach (var id in patientIDs)
                {
                    var a = GetPatients(id).SingleOrDefault();
                    if (a != null)
                    {
                        elements.Add(a);
                    }
                }
                CacheRepository.SetAll(elements);
            }
        }


        public void Remove(int patientID)
        {
            CacheRepository.Remove(patientID);
        }


        private IEnumerable<PatientsListItem2VM> GetPatients(int? patientId)
        {
            var sql = "EXEC dbo.GetPatientSearch";
            if (patientId.HasValue)
            {
                return Context.Database.SqlQuery<PatientsListItem2VM>($"{sql} @patientID", new SqlParameter("patientID", patientId.Value)).ToList();
            }
            return Context.Database.SqlQuery<PatientsListItem2VM>(sql).ToList();
        }

    }
}
