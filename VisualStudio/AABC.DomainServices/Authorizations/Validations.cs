namespace AABC.DomainServices.Authorizations
{
    public class Validations
    {


        /// <summary>
        /// Determine if the specified Authorization can be deleted.
        /// </summary>
        /// <param name="authID">ID of the authorization to check</param>
        /// <returns>true if deletable, false if not</returns>
        public static bool AuthorizationIsDeletable(int authID) {

            var service = new Data.Services.DataListService();
            
            if (service.GetInsuranceCountByAuthorizationID(authID) > 0) {
                return false;
            }
            
            if (service.GetCaseCountByAuthorizationID(authID) > 0) {
                return false;
            }

            return true;
        }


    }
}
