using AABC.Domain.Admin;
using System;
using System.Collections.Generic;

namespace AABC.Data.Mappings
{
    public static class WebUserMappings
    {
        
        public static User User(Models.WebUser entity) {

            var u = new User();

            u.AspNetUserID = entity.AspNetUserID;
            u.DateCreated = entity.DateCreated;
            u.Email = entity.WebUserEmail;
            u.FirstName = entity.WebUserFirstName;
            u.ID = entity.ID;
            u.LastName = entity.WebUserLastName;
            u.StaffMember = OfficeStaffMapping.OfficeStaff(entity.Staff);
            u.UserName = entity.UserName;

            return u;

        }


        public static List<UserPermission> Permissions(List<Models.WebUserPermission> entities) {
            var list = new List<UserPermission>();
            foreach (var entity in entities) {
                list.Add(WebUserMappings.Permission(entity));
            }
            return list;
        }

        public static List<UserOption> Options(List<Models.WebUserOption> entities) {
            var list = new List<UserOption>();
            foreach (var entity in entities) {
                list.Add(WebUserMappings.Option(entity));
            }
            return list;
        }


        public static UserOption Option(Models.WebUserOption entity) {
            var opt = new UserOption();
            opt.Description = entity.WebOption.WebOptionDescription;
            opt.ID = entity.WebOptionID.Value;
            opt.OptionValue = entity.WebOptionValue;
            opt.UserOptionID = entity.ID;
            return opt;
        }

        public static UserPermission Permission(Models.WebUserPermission entity) {
            var p = new UserPermission();
            p.ID = (Domain.Admin.Permissions)Enum.Parse(typeof(Domain.Admin.Permissions), entity.WebPermissionID.ToString());
            p.UserPermissionID = entity.ID;
            p.isAllowed = entity.isAllowed == null ? false : entity.isAllowed.Value;
            return p;
        }


    }
}
