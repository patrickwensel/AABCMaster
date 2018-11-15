using AABC.Domain2.PatientPortal;
using System;
using System.Linq;
using System.Web.Security;

namespace AABC.PatientPortal.Infrastructure.Membership
{

    public interface IMembershipRepository
    {
        MembershipUser GetUser(string username);
        string GetPassword(string username);
        int CreateUserAndAccount(string username, string password, string email);
        bool ChangePassword(string username, string newPassword);
    }


    public class MembershipRepository : IMembershipRepository
    {


        /*************************
        * 
        * FIELDS
        * 
        ************************/

        const string DEFAULT_PROVIDER_NAME = "DymengMembershipProvider";

        private string membershipProviderName;

        /*************************
        * 
        * CONSTRUCTOR
        * 
        ************************/

        public MembershipRepository() {
            membershipProviderName = DEFAULT_PROVIDER_NAME;
        }

        public MembershipRepository(
            string membershipProviderName
            ) {

            this.membershipProviderName = membershipProviderName;
        }

        /*************************
        * 
        * PUBLIC METHODS
        * 
        ************************/


        public bool ChangePassword(string email, string newPassword) {

            try {
                var context = AppService.Current.Data.Context;
                var user = context.PatientPortalLogins.Where(x => x.Email == email).Single();

                user.WebMembershipDetail.Password = newPassword;

                context.SaveChanges();

                return true;
            }
            catch (Exception) {
                return false;
            }

        }

        public int CreateUserAndAccount(string username, string password, string email) {

            var user = new Login();
            var context = AppService.Current.Data.Context;
            
            user.Email = email;

            context.PatientPortalLogins.Add(user);
            context.SaveChanges();

            var membership = new WebMembershipDetail();
            membership.CreationDate = DateTime.UtcNow;
            membership.Password = password;
            membership.ID = user.ID;

            context.PatientPortalWebMembershipDetails.Add(membership);
            context.SaveChanges();

            return 1;

        }

        public string GetPassword(string email) {

            var context = AppService.Current.Data.Context;
            var user = context.PatientPortalLogins.Where(x => x.Email == email).FirstOrDefault();
            WebMembershipDetail membership = null;
            if (user != null) {
                membership = context.PatientPortalWebMembershipDetails.Where(x => x.ID == user.ID).FirstOrDefault();
            }
            
            return membership?.Password;

        }


        public MembershipUser GetUser(string email) {

            var context = AppService.Current.Data.Context;
            var user = context.PatientPortalLogins.Where(x => x.Email == email && x.Active == true).FirstOrDefault();
            WebMembershipDetail entity = null;

            if (user != null) {
                entity = context.PatientPortalWebMembershipDetails.Where(x => x.ID == user.ID).FirstOrDefault();
            }
            
            if (entity == null) {
                return new MembershipUser(
                    providerName: membershipProviderName,
                    name: "",
                    providerUserKey: null,
                    email: "",
                    passwordQuestion: "",
                    comment: "",
                    isApproved: false,
                    isLockedOut: true,
                    creationDate: DateTime.UtcNow,
                    lastLoginDate: DateTime.UtcNow,
                    lastActivityDate: DateTime.UtcNow,
                    lastPasswordChangedDate: DateTime.UtcNow,
                    lastLockoutDate: DateTime.UtcNow
                    );
            } else {
                return new MembershipUser(
                    providerName: membershipProviderName,
                    name: entity.User.Email,
                    providerUserKey: null,
                    email: entity.User.Email,
                    passwordQuestion: entity.PasswordQuestion,
                    comment: "",
                    isApproved: entity.IsApproved,
                    isLockedOut: entity.IsLockedOut,
                    creationDate: entity.CreationDate,
                    lastLoginDate: entity.LastLoginDate,
                    lastActivityDate: entity.LastActivityDate,
                    lastPasswordChangedDate: entity.LastPasswordChangeDate,
                    lastLockoutDate: entity.LastLockoutDate
                    );
            }

        }

        /*************************
        * 
        * PRIVATE METHODS
        * 
        ************************/



    }

}