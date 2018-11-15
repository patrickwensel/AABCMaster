using AABC.Domain.Email;
using AABC.DomainServices.Email;
using System;

namespace AABC.Web.App.Systems
{
    public class Mailer
    {
        private readonly SMTPAccount SMTPAccount;
        private readonly string Recipient;

        public Mailer(SMTPAccount smtpAccount, string recipient)
        {
            if (smtpAccount == null) throw new ArgumentNullException(nameof(smtpAccount));
            if (string.IsNullOrEmpty(recipient)) throw new ArgumentNullException(nameof(recipient));
            SMTPAccount = smtpAccount;
            Recipient = recipient;
        }

        public void SendNotification(string name, string phone, string email)
        {
            var subject = "RBT Course Payment";
            var message = $@"Name: {name}
Phone: {phone}
Email: {email}";
            SMTP.Send(SMTPAccount, subject, message, Recipient);
        }
    }
}