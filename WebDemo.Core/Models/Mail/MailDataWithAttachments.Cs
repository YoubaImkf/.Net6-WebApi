using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebDemo.Core.Models.Mail
{
    public class MailDataWithAttachments
    {
        // Receiver
        public List<string> To { get; set; }
        public List<string> Bcc { get; set; }
        public List<string> Cc { get; set; }

        // Sender
        public string From { get; set; }
        public string DisplayName { get; set; }
        public string ReplyTo { get; set; }
        public string ReplyToName { get; set; }

        // Content
        public string Subject { get; set; }
        public string Body { get; set; }
        public IFormFileCollection Attachments { get; set; }
    }
}
