using AABC.Domain.ProviderPortal;
using System.Collections.Generic;

namespace AABC.Data.Mappings
{
    public static class ProviderPortalMappings
    {

        public static ProviderPortalUser ProviderPortalUser(Models.ProviderPortalUser entity, bool includeProviderMap = false) {
            var p = new ProviderPortalUser();
            p.AspNetUserID = entity.AspNetUserID;
            p.DateCreated = entity.DateCreated;
            p.ID = entity.ID;
            p.ProviderUserNumber = entity.ProviderUserNumber;
            p.ProviderID = entity.ProviderID;
            if (includeProviderMap) {
                p.Provider = ProviderMappings.Provider(entity.Provider);
            }
            p.HasAppAccess = entity.ProviderHasAppAccess;
            return p;
        }


        public static IEnumerable<ProviderPortalUser> ProviderPortalUsers(IEnumerable<Models.ProviderPortalUser> entities) {
            var list = new List<ProviderPortalUser>();
            foreach (var entity in entities) {
                list.Add(ProviderPortalMappings.ProviderPortalUser(entity));
            }
            return list;
        }

    }
}
