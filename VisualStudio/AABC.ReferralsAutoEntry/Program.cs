using System;
using System.Configuration;
using System.Linq;

namespace AABC.ReferralsAutoEntry
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Running...");
            try {

                // if we supplied some args, assume they're for testing
                // and do not run the standard process
                // test processes will be fired from the startupArgs method
                if (!startupArgs(args)) {
                    new ReferralsFromEmail().Execute();
                }
                
            } catch (Exception e) {
                Logger logger = new Logger("program.cs");
                logger.LogError(e.ToString());
            }
            
            Console.WriteLine("Finished.");
        }

        // returns true if startup args were present
        static bool startupArgs(string[] args) {

            if (args == null) {
                return false;
            }

            if (args.Length == 0) {
                return false;
            }

            for (int i = 0; i < args.Length; i++) {
                string arg = args[i];
                switch (arg) {
                    case "test-email-pop":
                        testEmailPop();
                        return true;
                    case "test-email-smtp":
                        testEmailSmtp();
                        return true;
                    default:
                        break;
                }
            }

            return false;

        }

        static void testEmailPop() {

            string _popServer;
            int _popPort;
            bool _popSSL;

            string _popUser;
            string _popPassword;

            _popServer = ConfigurationManager.AppSettings["PopServer"];
            _popPort = int.Parse(ConfigurationManager.AppSettings["PopPort"]);
            _popSSL = bool.Parse(ConfigurationManager.AppSettings["PopSSL"]);

            _popUser = ConfigurationManager.AppSettings["PopUser"];
            _popPassword = ConfigurationManager.AppSettings["PopPassword"];

            try {
                using (var popClient = new OpenPop.Pop3.Pop3Client()) {
                    popClient.Connect(_popServer, _popPort, _popSSL);
                    popClient.Authenticate(_popUser, _popPassword);

                    Console.WriteLine("SUCCESS (pop client)");

                    popClient.Disconnect();
                }
            } catch (Exception e) {
                Console.WriteLine("FAIL (pop client)");
                Console.WriteLine(e.ToString());
            }

            
                

        }
        
        static void testEmailSmtp() {

            AABC.Data.Models.CoreEntityModel _context;
            AABC.Domain.Email.SMTPAccount _smtpAccount;
            string _errorEmail;
            
            
            _context = new AABC.Data.Models.CoreEntityModel();
            _errorEmail = ConfigurationManager.AppSettings["ErrorEmail"];

            var smtpInfo = _context.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();

            if (smtpInfo == null) {
                Console.WriteLine("FAIL: (smtp) Unable to load Primary smtp account");
                return;
            }

            _smtpAccount = new AABC.Domain.Email.SMTPAccount()
            {
                Username = smtpInfo.AccountUsername,
                Password = smtpInfo.AccountPassword,
                Server = smtpInfo.AccountServer,
                Port = smtpInfo.AccountPort.Value,
                UseSSL = smtpInfo.AccountUseSSL.Value,
                AuthenticationMode = smtpInfo.AccountAuthMode.Value,
                FromAddress = smtpInfo.AccountDefaultFromAddress,
                ReplyToAddress = smtpInfo.AccountDefaultReplyAddress
            };

            try {

                AABC.DomainServices.Email.SMTP.Send(
                    _smtpAccount,
                    "Test Message", "This is a test message from auto referrals entry - please disregard.",
                    _errorEmail);

            } catch (Exception e) {
                Console.WriteLine("FAIL: (smtp)");
                Console.WriteLine(e.ToString());
                return;
            }
            
            Console.WriteLine("SUCCESS (smtp)");

        }

    }
}
