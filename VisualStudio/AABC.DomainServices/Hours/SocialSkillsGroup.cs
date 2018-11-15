using System.Linq;

namespace AABC.DomainServices.Hours
{
    public class SocialSkillsGroup
    {


        private static Data.V2.CoreContext _context;

        public SocialSkillsGroup() {
            _context = ContextProvider.Context;
        }
        public SocialSkillsGroup(Data.V2.CoreContext context) {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Case IDs for associated SSG entries, or null if not an SSG entry
        /// </summary>
        /// <param name="hoursID"></param>
        /// <returns></returns>
        public static int[] GetAssociatedCaseIDs(int hoursID) {

            if (_context == null) {
                _context = ContextProvider.Context;
            }

            var entry = _context.Hours.Find(hoursID);
            if (entry.SSGParentID.HasValue) {
                return _context.Hours.Where(x => x.SSGParentID == entry.SSGParentID.Value).Select(x => x.CaseID).ToArray();
            } else {
                return null;
            }
        }


        /// <summary>
        /// True if the specified hoursID is an SSG entry, and if this is the entry that the SSG was originated with
        /// </summary>
        /// <param name="hoursID"></param>
        /// <returns></returns>
        public static bool IsOriginatingHoursEntry(int hoursID) {

            if (_context == null) {
                _context = ContextProvider.Context;
            }

            var entry = _context.Hours.Find(hoursID);

            if (entry.SSGParentID.HasValue) {
                if (entry.SSGParentID.Value == entry.ID) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

    }
}
