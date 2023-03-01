using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

// SOURCE: https://blog.christian-schou.dk/send-emails-with-asp-net-core-with-mailkit/

namespace WebDemo.Core.Models.Mail
{
    public class MailData
    {
        // Receiver
        public List<string> To { get; }
        public List<string> Bcc { get; }

        public List<string> Cc { get; }

        // Sender
        public string From { get; }

        public string DisplayName { get; }

        public string ReplyTo { get; }

        public string ReplyToName { get; }

        // Content
        public string Subject { get; }

        public string Body { get; }

        // Attachments
        public List<IFormFile> Attachments { get; }

        public MailData(List<string> to, string subject, string body = null, string from = null, string displayName = null, string replyTo = null, string replyToName = null, List<string> bcc = null, List<string> cc = null)
        {
            // Receiver
            To = to;
            Bcc = bcc ?? new List<string>();
            Cc = cc ?? new List<string>();

            // Sender
            From = from;
            DisplayName = displayName;
            ReplyTo = replyTo;
            ReplyToName = replyToName;

            // Content
            Subject = subject;
            Body = body;

            // Attachments
            Attachments = Attachments ?? new List<IFormFile>();
        }
    }
}
