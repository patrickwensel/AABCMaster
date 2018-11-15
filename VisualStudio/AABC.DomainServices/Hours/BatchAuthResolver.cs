using AABC.DomainServices.HoursResolution;
using AABC.DomainServices.HoursResolution.Logging;
using AABC.DomainServices.HoursResolution.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Hours
{
    public class BatchAuthResolver
    {
        public delegate void AuthResolvedEventHandler(object sender, AuthResolvedEventArgs e);
        public event AuthResolvedEventHandler AuthResolved;
        public delegate void ExceptionBypassedEventHandler(object sender, ExceptionBypassedEventArgs e);
        public event ExceptionBypassedEventHandler ExceptionBypassed;

        private readonly Data.V2.CoreContext Context;

        public BatchAuthResolver(Data.V2.CoreContext context)
        {
            Context = context;
        }

        public int ResolveAllAuths(List<int> hourIDs)
        {
            var hours = Context.Hours.Join(hourIDs, h => h.ID, id => id, (h, id) => h).ToList();
            return Resolve(hours);
        }


        public int ResolveAllAuths(DateTime startDate, DateTime endDate)
        {
            var hours = Context.Hours.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            return Resolve(hours);
        }


        private int Resolve(IEnumerable<Domain2.Hours.Hours> hours)
        {
            int count = 0;
            int loopCount = 0;
            int totalCount = hours.Count();
            var resolver = new AuthorizationResolution(new ResolutionServiceRepository(Context), HoursResolutionLoggingService.Create());
            foreach (var h in hours)
            {
                try
                {
                    resolver.Resolve(new List<Domain2.Hours.Hours> { h });
                    Context.SaveChanges();
                    onAuthResolved(totalCount, loopCount, h.ID);
                    count += h.AuthorizationBreakdowns != null ? h.AuthorizationBreakdowns.Count : 0;
                }
                catch (Exception e)
                {
                    onExceptionBypassed(h.ID, e);
                }
                loopCount++;
                System.Diagnostics.Debug.WriteLine("LOOP: " + loopCount + " of " + totalCount);
            }
            return count;
        }


        private void onAuthResolved(int totalCount, int index, int hoursID)
        {
            AuthResolved?.Invoke(this, new AuthResolvedEventArgs(totalCount, index, hoursID));
        }


        private void onExceptionBypassed(int hoursID, Exception e)
        {
            ExceptionBypassed?.Invoke(this, new ExceptionBypassedEventArgs(hoursID, e));
        }


        public class AuthResolvedEventArgs : EventArgs
        {
            public int TotalAuths { get; private set; }
            public int CurrentIndex { get; private set; }
            public int HoursID { get; private set; }

            public AuthResolvedEventArgs(int totalAuths, int index, int hoursID)
            {
                TotalAuths = totalAuths;
                CurrentIndex = index;
                HoursID = hoursID;
            }
        }


        public class ExceptionBypassedEventArgs : EventArgs
        {
            public int HoursID { get; private set; }
            public Exception Exception { get; private set; }

            public ExceptionBypassedEventArgs(int hoursID, Exception e)
            {
                HoursID = hoursID;
                Exception = e;
            }
        }

    }
}
