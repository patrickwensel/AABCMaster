using System;

namespace AABC.Services
{

    class Debug
    {
        public static void Run() {
            // sandbox code
            

        }
    }


    public enum ExitCodes
    {
        Debug = -1,     
        Ok = 0,         
        GeneralError = 1,   
        CommandLineParserError = 2,
        VerbResolutionFailure = 3,
        OptionsValuesValidationFailure = 4,
        HandlerError = 5
    }



    class Program
    {
        
        public static int UserID { get { return 0; } }

        static void Main(string[] args) {

            //Debug.Run();
            //return;

            bool debug = false;
            string verb = "";
            object instance = null;
            var options = new VerbConfig();

#if DEBUG
            //args = new[] { "hours.authbreakdowns.reconcile", "-w", "-s", "2017-03-08", "-e", "2017-03-10" };
            debug = true;
#endif

            bool parsed = CommandLine.Parser.Default.ParseArguments(args, options, (v, i) => {
                verb = v;
                instance = i;
            });
            
            if (!parsed) {
                if (debug) {
                    Debug.Run();    // sandbox mode if debug mode and no valid args supplied
                    Environment.Exit((int)ExitCodes.Debug);
                } else {
                    Environment.Exit((int)ExitCodes.CommandLineParserError);
                }
            }

            if (!((VerbOptionsBase)instance).ValidateInput()) {
                Environment.Exit((int)ExitCodes.OptionsValuesValidationFailure);
            }

            int exitCode = handleVerb(verb, instance as VerbOptionsBase, debug);

            if (debug || ((VerbOptionsBase)instance).WaitOnExit) {
                Console.WriteLine("Exiting with code " + exitCode);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }

            Environment.Exit(exitCode);
            
        }


        static int handleVerb(string verb, VerbOptionsBase optionsBase, bool debug) {

            int exitCode = (int)ExitCodes.GeneralError;

            VerbHandlerBase handler = null;

            switch (verb) {

                case "hours.authbreakdowns.reconcile":
                    handler = new Hours.AuthBreakdowns.ReconcileHandler(optionsBase.Transform() as Hours.AuthBreakdowns.ReconcileOptions);
                    break;

                default:
                    exitCode = (int)ExitCodes.VerbResolutionFailure;
                    break;
            }

            if (handler != null) {

                handler.AddMessageHandler(new MessageHandlers.ConsoleOutputMessageHandler());

                if (debug) {
                    handler.AddMessageHandler(new MessageHandlers.IDEOutputMessageHandler());
                }

                exitCode = handler.Run();
            }

            return exitCode;

        }


    }


    

}
