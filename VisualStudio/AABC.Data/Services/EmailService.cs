using System.Linq;

namespace AABC.Data.Services
{
    public class EmailService
    {



        public Domain.Email.SMTPAccount GetSMTPAccount(string accountName) {

            var entity = new Models.CoreEntityModel().SMTPAccounts.Where(x => x.AccountName == accountName).FirstOrDefault();
            if (entity == null) {
                return null;
            }

            var acct = new Domain.Email.SMTPAccount();
            acct.AuthenticationMode = ((int?)entity.AccountAuthMode) ?? 1;
            acct.DisplayName = entity.AccountDisplayName;
            acct.FromAddress = entity.AccountDefaultFromAddress;
            acct.ID = entity.ID;
            acct.Name = entity.AccountName;
            acct.Password = entity.AccountPassword;
            acct.Port = (int?)entity.AccountPort ?? 25;
            acct.ReplyToAddress = entity.AccountDefaultReplyAddress;
            acct.Server = entity.AccountServer;
            acct.Username = entity.AccountUsername;
            acct.UseSSL = entity.AccountUseSSL ?? true;

            return acct;

        }

    }
}
