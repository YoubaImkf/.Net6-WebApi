using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebDemo.Core.Models.Mail;

// SOURCE: https://blog.christian-schou.dk/send-emails-with-asp-net-core-with-mailkit/

namespace WebDemo.Core.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
        Task<bool> SendWithAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct);
    }
}
