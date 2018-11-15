using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Services
{
    public class DataListService
    {


        public int GetCaseCountByAuthorizationID(int authID) {
            var context = new Models.CoreEntityModel();
            return context.CaseAuthCodes.Where(x => x.AuthCodeID == authID).Count();
        }

        public int GetInsuranceCountByAuthorizationID(int authID) {
            return 0;
            //var context = new Models.CoreEntityModel();
            //return context.InsuranceAuths.Where(x => x.AuthCodeID == authID).Count();
        }


        public Domain.Cases.Authorization GetAuthorization(int id) {
            var context = new Models.CoreEntityModel();
            var entity = context.AuthCodes.Find(id);
            return Mappings.AuthorizationMappings.Authorization(entity);
        }

        public Domain.Cases.Authorization GetAuthorization(string authCode) {
            var context = new Models.CoreEntityModel();
            var entity = context.AuthCodes.Where(x => x.CodeCode == authCode).FirstOrDefault();
            return Mappings.AuthorizationMappings.Authorization(entity);
        }

        public List<Domain.Cases.Authorization> GetAuthorizations() {

            var context = new Models.CoreEntityModel();

            var entities = context.AuthCodes.ToList();

            return Mappings.AuthorizationMappings.Authorizations(entities);

        }

    }
}
