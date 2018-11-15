using System;

namespace AABC.Domain.Admin
{
    public class UserPermission : Permission
    {
        public int? UserPermissionID { get; set; }
        public DateTime UserPermissionDateCreated { get; set; }
        public Permission Permission { get; set; }
        public bool isAllowed { get; set; }

        public UserPermission() {
            UserPermissionDateCreated = DateTime.UtcNow;
        }
    }
}
