using AABC.DomainServices.Hours;
using System;

namespace AABC.Services.Hours.AuthBreakdowns
{
    public class ReconcileHandler : VerbHandlerBase
    {

        private ReconcileOptions _options;

        public ReconcileHandler(ReconcileOptions options)
        {
            _options = options;
        }

        public override int Run()
        {

            try
            {
                var resolver = new BatchAuthResolver(new Data.V2.CoreContext());

                resolver.AuthResolved += Resolver_AuthResolved;
                resolver.ExceptionBypassed += Resolver_ExceptionBypassed;

                resolver.ResolveAllAuths(_options.StartDate, _options.EndDate);

                return (int)ExitCodes.Ok;

            }
            catch (Exception e)
            {

                this.PostMessageLine(e.ToString());
                return (int)ExitCodes.HandlerError;
            }

        }

        private void Resolver_ExceptionBypassed(object sender, BatchAuthResolver.ExceptionBypassedEventArgs e)
        {

            this.PostMessageLine("-----------------------");
            this.PostMessageLine("EXCEPTION BYPASSED: HID=" + e.HoursID + ", HRESULT=" + e.Exception.HResult);
            this.PostMessageLine("");
            this.PostMessageLine(e.ToString());
            this.PostMessageLine("-----------------------");

            if (_options.BreakOnExceptionBypassed)
            {

                this.PostMessageLine("");
                this.PostMessageLine("Break: press any key to continue");
                Console.ReadKey();
            }

        }

        private void Resolver_AuthResolved(object sender, BatchAuthResolver.AuthResolvedEventArgs e)
        {
            this.PostMessageLine(e.CurrentIndex + 1 + " of " + e.TotalAuths + " (HID: " + e.HoursID + ")");
        }
    }
}
