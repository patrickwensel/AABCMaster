
using AABC.Domain.OfficeStaff;

namespace AABC.Data.Mappings
{
    public static class OfficeStaffMapping
    {


        public static OfficeStaff OfficeStaff(Models.Staff entity) {
            
            if (entity == null) {
                return null;
            }

            var s = new OfficeStaff();

            s.Active = entity.StaffActive;
            s.DateCreated = entity.DateCreated;
            s.DateCreated = entity.DateCreated;
            s.Email = entity.StaffPrimaryEmail;
            s.FirstName = entity.StaffFirstName;
            s.HireDate = entity.StaffHireDate;
            s.ID = entity.ID;
            s.LastName = entity.StaffLastName;
            s.Phone = entity.StaffPrimaryPhone;
            s.TerminationDate = entity.StaffTerminatedDate;

            return s;

        }

    }
}
