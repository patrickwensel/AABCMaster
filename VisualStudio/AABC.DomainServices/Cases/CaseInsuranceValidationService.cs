using AABC.Data.V2;
using System;
using System.Linq;

namespace AABC.DomainServices.Cases
{
    public class CaseInsuranceValidationService
    {

        private readonly CoreContext _context;

        public CaseInsuranceValidationService(CoreContext context)
        {
            _context = context;
        }

        public bool ValidateInsuranceUpdate(int caseID, Domain2.Cases.CaseInsurance caseInsurance)
        {
            DateTime fullRangeStart = DateTime.MinValue;
            DateTime fullRangeEnd = DateTime.MaxValue;
            if (caseInsurance.DatePlanEffective.GetValueOrDefault(fullRangeStart) >= caseInsurance.DatePlanTerminated.GetValueOrDefault(fullRangeEnd))
            {
                throw new InvalidOperationException("Effective date cannot be later than termination date");
            }
            var target = _context.CaseInsurances.Where(m => m.CaseID == caseID && m.ID != caseInsurance.ID).ToList();
            target.Add(caseInsurance);

            // convert null effective/defective dates to outer ranges
            var tuples = target.Select(t =>
            {
                var start = t.DatePlanEffective.GetValueOrDefault(fullRangeStart);
                var end = t.DatePlanTerminated.GetValueOrDefault(fullRangeEnd);
                return new Tuple<DateTime, DateTime>(start, end);
            });
            return !AreAnyOverlaps(tuples.ToArray());
        }

        private static bool AreAnyOverlaps(params Tuple<DateTime, DateTime>[] ranges)
        {
            for (int i = 0; i < ranges.Length; i++)
            {
                for (int j = i + 1; j < ranges.Length; j++)
                {
                    if (ranges[i].Item1 <= ranges[j].Item2 && ranges[i].Item2 >= ranges[j].Item1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
