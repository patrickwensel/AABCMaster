namespace AABC.Domain.Email
{
    public class SMTPAccount
    {

        public int? ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public int AuthenticationMode { get; set; }
        public string ReplyToAddress { get; set; }
        public string FromAddress { get; set; }

    }
}
