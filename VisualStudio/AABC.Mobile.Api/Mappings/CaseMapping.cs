using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AABC.Domain2.Cases;
using AABC.Mobile.SharedEntities.Entities;

namespace AABC.Mobile.Api.Mappings
{
    class CaseMapping : IDomainMapperReadonly<Domain2.Cases.Case, SharedEntities.Entities.Case>
    {

        InsuranceMapping insuranceMapping = new InsuranceMapping();
        PatientMapping patientMapping = new PatientMapping();
        ActiveAuthorizationMapping authMapping = new ActiveAuthorizationMapping();
        

        public SharedEntities.Entities.Case FromDomain(Domain2.Cases.Case source) {

            var result = new SharedEntities.Entities.Case();

            result.ID = source.ID;
            result.Patient = getPatient(source.Patient);
            result.ActiveAuthorizations = getActiveAuthorizations(source.GetActiveAuthorizations());
            result.ActiveInsurance = getActiveInsurance(source.GetActiveInsuranceAtDate(DateTime.Now));
            
            return result;
        }



        private List<ActiveAuthorization> getActiveAuthorizations(IEnumerable<Domain2.Authorizations.Authorization> auths) {

            var result = new List<ActiveAuthorization>();

            if (auths == null || auths.Count() == 0) {
                return result;
            }
            foreach (var auth in auths) {
                result.Add(authMapping.FromDomain(auth));
            }
            return result;
        }



        private Patient getPatient(Domain2.Patients.Patient patient) {            
            if (patient == null) {
                throw new ArgumentNullException("patient");
            }
            return patientMapping.FromDomain(patient);
        }



        private Insurance getActiveInsurance(CaseInsurance caseInsurance) {
            if (caseInsurance == null || caseInsurance.Insurance == null) {
                return null;
            }
            return insuranceMapping.FromDomain(caseInsurance.Insurance);
        }

        

        

    }
}