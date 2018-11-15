using AABC.Data.Models;
using AABC.Domain.Cases;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Data.Mappings
{
    public static class AuthorizationMappings
    {





        /*********************
         * CASE AUTHS
         *********************/

        public static CaseAuthorization CaseAuthorization(Models.CaseAuthCode entity) {

            if (entity == null) {
                return null;
            }

            var auth = new CaseAuthorization();
            var baseAuth = AuthorizationMappings.Authorization(entity.AuthCode);
                        
            auth.AuthClass = AuthorizationMappings.AuthorizationClass(entity.CaseAuthClass);
            auth.CaseAuthorizationDateCreated = entity.DateCreated;
            auth.CaseAuthorizationID = entity.ID;
            auth.Code = baseAuth.Code;
            auth.DateCreated = baseAuth.DateCreated;
            auth.Description = baseAuth.Description;
            auth.EndDate = entity.AuthEndDate;
            auth.ID = entity.ID;
            auth.StartDate = entity.AuthStartDate;
            auth.TotalHoursApproved = (double)entity.AuthTotalHoursApproved;

            return auth;           

        }


        public static Authorization Authorization(Models.AuthCode entity) {
            if (entity == null) {
                return null;
            }
            var auth = new Authorization();
            auth.Code = entity.CodeCode;
            auth.DateCreated = entity.DateCreated;
            auth.Description = entity.CodeDescription;
            auth.ID = entity.ID;
            return auth;
        }


        public static List<Authorization> Authorizations(List<Models.AuthCode> entities) {
            var list = new List<Authorization>();
            foreach (var entity in entities) {
                list.Add(Mappings.AuthorizationMappings.Authorization(entity));
            }
            return list;
        }


        public static AuthorizationClass AuthorizationClass(Models.CaseAuthClass entity) {
            var authClass = new AuthorizationClass();
            authClass.Code = entity.AuthClassCode;
            authClass.Description = entity.AuthClassDescription;
            authClass.ID = entity.ID;
            authClass.Name = entity.AuthClassName;
            return authClass;
        }


        /*********************
         * CASE AUTH HOURS
         *********************/
         

        public static List<CaseAuthorizationHours> CaseAuthorizationHours(List<CaseAuthHour> entities) {

            if (entities == null) {
                return null;
            }

            var serviceLocations = new Services.ServicesService().GetActiveServiceLocations();

            var hours = new List<CaseAuthorizationHours>();
            foreach(var entity in entities) {
                hours.Add(AuthorizationMappings.CaseAuthorizationHour(entity, serviceLocations));
            }
            return hours;

        }


        public static CaseAuthorizationHours CaseAuthorizationHour(CaseAuthHour entity, List<Domain.Services.ServiceLocation> serviceLocations = null) {

            var hours = new CaseAuthorizationHours();

            hours.Date = entity.HoursDate;
            hours.DateCreated = entity.DateCreated;
            hours.Status = (AuthorizationHoursStatus)entity.HoursStatus;
            hours.CaseID = entity.CaseID;
            hours.HoursTotal = (double)entity.HoursTotal;
            hours.ID = entity.ID;
            hours.Notes = entity.HoursNotes;
            hours.TimeIn = hours.Date + entity.HoursTimeIn;
            hours.TimeOut = hours.Date + entity.HoursTimeOut;
            if (entity.HoursServiceID.HasValue) {
                hours.Service = new Data.Services.CaseService().GetServices().Where(x => x.ID == entity.HoursServiceID.Value).FirstOrDefault();
            }
            hours.Provider = new Domain.Cases.CaseProvider() { ID = entity.CaseProviderID };
            hours.Authorization = new CaseAuthorization() { ID = entity.CaseAuthID };
            hours.PayableHours = (double?)entity.HoursPayable;
            hours.BillableHours = (double?)entity.HoursBillable;
            hours.BillingRef = entity.HoursBillingRef;
            hours.PayableRef = entity.HoursPayableRef;
            hours.ProviderID = entity.CaseProviderID;
            hours.HasCatalystData = entity.HoursHasCatalystData;
            hours.WatchEnabled = entity.HoursWatchEnabled;
            hours.WatchNote = entity.HoursWatchNote;
            hours.SSGParentID = entity.HoursSSGParentID;
            hours.CorrelationID = entity.HoursCorrelationID;
            hours.InternalNotes = entity.HoursInternalNotes;
            hours.IsPayrollOrBillingAdjustment = entity.IsPayrollOrBillingAdjustment;
            hours.ServiceLocationID = entity.ServiceLocationID;

            if (entity.ServiceLocationID.HasValue) {
                if (serviceLocations == null) {
                    hours.ServiceLocation = new Services.ServicesService().GetActiveServiceLocations().Where(x => x.ID == entity.ServiceLocationID).FirstOrDefault();
                } else {
                    hours.ServiceLocation = serviceLocations.Where(x => x.ID == entity.ServiceLocationID).FirstOrDefault();
                }
            }

            return hours;
            
        }

        
        



        /*********************
         * CASE AUTH GENERAL HOURS
         *********************/

        public static CaseAuthorizationGeneralHours CaseAuthorizationGeneralHour(Models.CaseAuthCodeGeneralHour entity) {
            if (entity == null) {
                return null;
            }

            var item = new CaseAuthorizationGeneralHours();
            item.CaseAuthID = entity.CaseAuthID;
            item.DateCreated = entity.DateCreated;
            item.Hours = (double)entity.HoursApplied;
            item.ID = entity.ID;
            item.Month = entity.HoursMonth;
            item.Year = entity.HoursYear;

            return item;

        }

        public static List<CaseAuthorizationGeneralHours> CaseAuthorizationGeneralHours(List<Models.CaseAuthCodeGeneralHour> entities) {
            if (entities == null) {
                return null;
            }
            var generalHours = new List<CaseAuthorizationGeneralHours>();
            foreach (var entity in entities) {
                generalHours.Add(AuthorizationMappings.CaseAuthorizationGeneralHour(entity));
            }
            return generalHours;
        }

    }
}
