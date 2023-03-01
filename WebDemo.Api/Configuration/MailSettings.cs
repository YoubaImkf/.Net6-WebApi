namespace WebDemo.Api.Configuration
{
    // SOURCE: https://blog.christian-schou.dk/send-emails-with-asp-net-core-with-mailkit/
    public class MailSettings
    {
        public string DisplayName { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
    }
}
