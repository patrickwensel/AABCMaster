using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AABC.ReferralsAutoEntry
{
    class ReferralsFromEmail
    {
        AABC.Data.Models.CoreEntityModel _context;
        Logger _logger;

        string _popServer;
        int _popPort;
        bool _popSSL;

        string _popUser;
        string _popPassword;

        AABC.Domain.Email.SMTPAccount _smtpAccount;
        string _validationEmail;
        string _errorEmail;

        bool _seedMode;

        public ReferralsFromEmail()
        {
            _context = new AABC.Data.Models.CoreEntityModel();
            _logger = new Logger();

            _popServer = ConfigurationManager.AppSettings["PopServer"];
            _popPort = int.Parse(ConfigurationManager.AppSettings["PopPort"]);
            _popSSL = bool.Parse(ConfigurationManager.AppSettings["PopSSL"]);

            _popUser = ConfigurationManager.AppSettings["PopUser"];
            _popPassword = ConfigurationManager.AppSettings["PopPassword"];

            _seedMode = bool.Parse(ConfigurationManager.AppSettings["SeedMode"]);

            var smtpInfo = _context.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();

            if(smtpInfo == null) {
                throw new ArgumentNullException("Primary SMTP Account info has not been configured.");
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

            _validationEmail = ConfigurationManager.AppSettings["ValidationEmail"];
            _errorEmail = ConfigurationManager.AppSettings["ErrorEmail"];

        }

        public bool Execute()
        {

            // Get already processed messages list
            List<String> processedMessages = _context.ReferralEmails.Select(x => x.MessageID).ToList();
            if (processedMessages == null) {
                processedMessages = new List<string>();
            }

            // Get any un-processed messages
            OpenPop.Pop3.Pop3Client popClient = new OpenPop.Pop3.Pop3Client();
            popClient.Connect(_popServer, _popPort, _popSSL);
            popClient.Authenticate(_popUser, _popPassword);

            var messageIDs = popClient.GetMessageUids();
            for(int i = 0; i < messageIDs.Count; i++)
            {
                if (!processedMessages.Contains(messageIDs[i]))
                {
                    OpenPop.Mime.Message msg = null;
                    string msgStatus = "";
                    string msgSubject = "";
                    try
                    {
                        msg = popClient.GetMessage(i + 1);
                        msgSubject = msg.Headers.Subject;
                        if (!_seedMode) {
                            msgStatus = ProcessMessage(msg);
                        }
                    }
                    catch(System.Data.Entity.Validation.DbEntityValidationException ex) {

                        if(ex.EntityValidationErrors != null
                            && ex.EntityValidationErrors.Count() > 0
                            && ex.EntityValidationErrors.First().ValidationErrors != null
                            && ex.EntityValidationErrors.First().ValidationErrors.Count > 0)
                        {
                            string errorMessage = "An error occurred validating the referral."
                                + "\r\nID: " + messageIDs[i]
                                + "\r\nSubject: " + msgSubject
                                + "\r\nFirst Validation Error: " + ex.EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;

                            _logger.LogError(errorMessage);
                            AABC.DomainServices.Email.SMTP.Send(_smtpAccount, "Referral Validation Error", errorMessage, _validationEmail);

                        }

                        else
                        {
                            string errorMessage = "An error occurred validating the referral." 
                                + "\r\nID: " + messageIDs[i]
                                + "\r\nSubject: " + msgSubject
                                + "\r\nError Message: " + ex.Message;

                            _logger.LogError(errorMessage);
                            AABC.DomainServices.Email.SMTP.Send(_smtpAccount, "Referral Validation Error", errorMessage, _validationEmail);
                        }

                        msgStatus = "Validation Error";

                    }
                    catch(System.Exception ex)
                    {
                        string errorMessage = "An error occurred processing the referral."
                            + "\r\nID: " + messageIDs[i]
                            + "\r\nSubject: " + msgSubject
                            + "\r\nError Message: " + ex.Message;

                        _logger.LogError(errorMessage);
                        AABC.DomainServices.Email.SMTP.Send(_smtpAccount, "Referral Error", errorMessage, _errorEmail);

                        msgStatus = "Errored";
                    }

                    _context.ReferralEmails.Add(new Data.Models.ReferralEmail()
                    {
                        MessageID = messageIDs[i],
                        MessageSubject = msgSubject,
                        MessageStatus = msgStatus,
                        ProcessedTime = DateTime.Now
                    });
                    _context.SaveChanges();

                }
            }

            popClient.Disconnect();
            popClient.Dispose();

            return true;
        }

        private string ProcessMessage(OpenPop.Mime.Message msg)
        {
            if (msg.Headers.ContentType.MediaType != "text/plain")
            {
                _logger.LogMessage("Skipped message with unexpected media type. Subject: " + msg.Headers.Subject);
                return "Skipped";
            }

            string bodyText = msg.MessagePart.BodyEncoding.GetString(msg.MessagePart.Body);
            List<String> lines = bodyText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            lines.RemoveAt(lines.IndexOf("Please see the intake referral from the website:"));
            int endIndex = lines.IndexOf("--");
            lines.RemoveRange(endIndex, lines.Count - endIndex);

            var referralInfo = new Dictionary<string, string>();
            foreach(string line in lines)
            {
                string[] items = line.Split(new string[] { ": "}, 2, StringSplitOptions.None);
                referralInfo.Add(items[0], items[1]);
            }


            var tmpContext = new AABC.Data.Models.CoreEntityModel();
            tmpContext.Referrals.Add(new Data.Models.Referral()
            {
                DateCreated = System.DateTime.Now,
                ReferralFirstName = GetReferralInfo("Childs First Name", referralInfo),
                ReferralLastName = GetReferralInfo("Childs Last Name", referralInfo),
                ReferralDateOfBirth = GetDateOfBirth(GetReferralInfo("Date of Birth", referralInfo)),
                ReferralGender = GetGender(GetReferralInfo("Gender", referralInfo)),
                ReferralPrimarySpokenLangauge = GetReferralInfo("Language", referralInfo),
                ReferralGuardianFirstName = GetReferralInfo("Parent First Name", referralInfo),
                ReferralGuardianLastName = GetReferralInfo("Parent Last Name", referralInfo),
                ReferralGuardianRelationship = GetReferralInfo("Relationship to Child", referralInfo),
                ReferralAddress1 = GetReferralInfo("Address", referralInfo),
                ReferralAddress2 = GetReferralInfo("address", referralInfo),
                ReferralCity = GetReferralInfo("City", referralInfo),
                ReferralState = GetReferralInfo("State", referralInfo),
                ReferralZip = GetReferralInfo("Zip", referralInfo),
                ReferralPhone = GetReferralInfo("Phone", referralInfo),
                ReferralAreaOfConcern = GetReferralInfo("Area of Concern", referralInfo),
                ReferralSourceName = GetReferralInfo("Referred By", referralInfo),
                ReferralServicesRequested = GetReferralInfo("Services interested in", referralInfo),
                ReferralInsuranceCompanyName = GetReferralInfo("Insurance Name", referralInfo),
                ReferralInsuranceMemberID = GetReferralInfo("Member ID", referralInfo),
                ReferralInsurancePrimaryCardholderDateOfBirth = GetInsuranceDOB(GetReferralInfo("Insurance primary holder and DOB", referralInfo)),
                ReferralInsuranceCompanyProviderPhone = GetReferralInfo("Insurance Company Phone # for Providers to call", referralInfo)
            });

            tmpContext.SaveChanges();
            _logger.LogMessage("Created referral for message. Subject: " + msg.Headers.Subject);
            return "Processed";

        }




        private string GetReferralInfo(string key, Dictionary<string, string> info)
        {
            string retVal;
            if (info.TryGetValue(key, out retVal))
            {
                return retVal;
            }
            else
            {
                return "";
            }
        }

        private DateTime? GetDateOfBirth(string dob)
        {
            DateTime res;
            if (DateTime.TryParse(dob, out res))
            {
                return res;
            }
            return null;
        }

        private string GetGender(string gender)
        {
            if(gender.Equals("male", StringComparison.CurrentCultureIgnoreCase) || gender.Equals("m", StringComparison.CurrentCultureIgnoreCase))
            {
                return "M";
            }

            if (gender.Equals("female", StringComparison.CurrentCultureIgnoreCase) || gender.Equals("f", StringComparison.CurrentCultureIgnoreCase))
            {
                return "F";
            }

            return null;

        }

        private DateTime? GetInsuranceDOB(string dob)
        {
            int spaceIndex = dob.LastIndexOf(' ');
            string substr = dob.Substring(spaceIndex + 1);
            return GetDateOfBirth(substr);
        }


    }
}
