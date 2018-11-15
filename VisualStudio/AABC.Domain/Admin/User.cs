using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Domain.Admin
{
    public class User
	{
		public int? ID { get; set; }
		public DateTime? DateCreated { get; set; }

		public int AspNetUserID { get; set; }
        public int? StaffMemberID { get; set; }
		public OfficeStaff.OfficeStaff StaffMember { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }

        public bool HasPermission(Permissions permission) {
            
            if (Permissions.Where(x => x.ID == permission && x.isAllowed).Count() > 0) {
                return true;
            } else {
                return false;
            }

        }

        public List<UserOption> Options { get; set; }
        public List<UserPermission> Permissions { get; set; }

		public User() {
			this.DateCreated = DateTime.UtcNow;
            Options = new List<UserOption>();
            Permissions = new List<UserPermission>();
		}
        

        public static string GenerateRandomPassword(int length = 8) {

            // removed difficult to discern letters and numbers (IiLlOo0)
            var chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++) {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

	}

	



}
