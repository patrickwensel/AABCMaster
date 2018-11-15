using System;

namespace AABC.Domain2.Authorizations
{

    // Must correspond with values in dbo.AuthMatchRules
    public enum BillingMethod {
        Timed = 0,
        Service = 1
    }
    

    public class AuthorizationMatchRule
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int InsuranceID { get; set; }
        public int ProviderTypeID { get; set; }
        public int ServiceID { get; set; }
        
        public int? InitialAuthorizationID { get; set; }
        public int? InitialMinimumMinutes { get; set; }
        public int? InitialUnitSize { get; set; }

        public int? FinalAuthorizationID { get; set; }
        public int? FinalMinimumMinutes { get; set; }
        public int? FinalUnitSize { get; set; }

        public BillingMethod BillingMethod { get; set; }
        public bool AllowOverlapping { get; set; }
        public bool RequiresAuthorizedBCBA { get; set; }
        public bool RequiresPreAuthorization { get; set; }


        public virtual Insurances.Insurance Insurance { get; set; }
        public virtual Providers.ProviderType ProviderType { get; set; }
        public virtual Services.Service Service { get; set; }

        public virtual AuthorizationCode InitialAuthorization { get; set; }
        public virtual AuthorizationCode FinalAuthorization { get; set; }



        public bool IsInitialAuthUsableForTimeEntry {
            get
            {
                if (InitialAuthorization == null) {
                    return false;
                }
                if (InitialMinimumMinutes.GetValueOrDefault(0) <= 0) {
                    return false;
                }
                if (InitialUnitSize.GetValueOrDefault(0) <= 0) {
                    return false;
                }
                return true;
            }
        }

        public bool IsFinalAuthUsableForTimeEntry {
            get
            {
                if (FinalAuthorization == null) {
                    return false;
                }
                if (FinalMinimumMinutes.GetValueOrDefault(0) <= 0) {
                    return false;
                }
                if (FinalUnitSize.GetValueOrDefault(0) <= 0) {
                    return false;
                }
                return true;
            }
        }

        public int GetApplicableInitialMinutes(int totalMinutes) {

            if (!IsInitialAuthUsableForTimeEntry) {
                return 0;
            }

            if (totalMinutes < InitialMinimumMinutes) {
                return 0;
            }

            int remainder = totalMinutes % InitialUnitSize.Value;
            int quotient = (totalMinutes / InitialUnitSize.Value) * InitialUnitSize.Value;

            int result = 0;
            
            if (remainder < InitialMinimumMinutes) {
                result = quotient;
            } else {
                result = quotient + InitialUnitSize.Value;
            }  
            
            // result can now be over the max, if the max is applicable
            if (FinalAuthorization != null && result > InitialUnitSize.Value) {
                result = InitialUnitSize.Value;
            }

            return result;

        }
        

        public int GetApplicableFinalMinutes(int totalMinutes) {

            int applicableTotalMinutes = totalMinutes - GetApplicableInitialMinutes(totalMinutes);

            if (applicableTotalMinutes < FinalMinimumMinutes) {
                return 0;
            }

            if (applicableTotalMinutes <= FinalUnitSize) {
                return FinalUnitSize.Value;
            }

            int remainder = applicableTotalMinutes % FinalUnitSize.Value;
            int quotient = (applicableTotalMinutes / FinalUnitSize.Value) * FinalUnitSize.Value;

            if (remainder < FinalMinimumMinutes) {
                return quotient;
            } else {
                return quotient + FinalUnitSize.Value;
            }


        }




    }
}
