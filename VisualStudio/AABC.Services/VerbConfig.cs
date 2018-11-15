
using CommandLine;

namespace AABC.Services
{

    /// <summary>
    /// Core configuration for top level command line verb parsing.
    /// For specific verb options, see the VerbOptionsBase and each
    /// Options class corresponding to the command handlers.
    /// </summary>
    public class VerbConfig
    {

        [VerbOption("hours.authbreakdowns.reconcile", HelpText = "Re-evaluates and re-applies all hours authorization breakdowns in the specified range")]
        public Hours.AuthBreakdowns.ReconcileVerbOptions HoursAuthBreakdownsVerb { get; set; }

    }
}
