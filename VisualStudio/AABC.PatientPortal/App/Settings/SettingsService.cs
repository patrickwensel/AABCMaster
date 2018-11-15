using AABC.PatientPortal.App.Settings.Models;
using System;
using System.Linq;

namespace AABC.PatientPortal.App.Settings
{
    public class SettingsService
    {
        internal void UpdateDisplayName(DisplayNameVM model) {

            var user = _context.PatientPortalLogins.Where(x => x.ID == model.UserID).SingleOrDefault();

            if (user == null) {
                throw new NullReferenceException("User should not be null");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            _context.SaveChanges();
                    
            // invalidate the user cache so it reloads the update details on next call    
            CacheService.Current.User.Remove(CacheService.Types.UserCache.Key.User);
                            
        }




        private Data.V2.CoreContext _context;

        public SettingsService() {
            _context = AppService.Current.Data.Context;
        }

        internal bool CheckPassword(string oldPassword) {

            var pw = AppService.Current.User.CurrentUser.WebMembershipDetail.Password;
            if (pw == oldPassword) {
                return true;
            } else {
                return false;
            }

        }

        internal void UpdatePassword(ChangePasswordVM model) {

            var detail = _context.PatientPortalWebMembershipDetails.Where(x => x.ID == model.UserID).Single();

            detail.Password = model.NewPassword;

            _context.SaveChanges();

            // invalidate the user cache so it reloads the update details on next call    
            CacheService.Current.User.Remove(CacheService.Types.UserCache.Key.User);
        }
    }
}