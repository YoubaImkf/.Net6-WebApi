using Microsoft.Extensions.Options;
using MimeKit;
using System.Linq;
using System.Net.Mail;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebDemo.Api.Configuration;
using WebDemo.Core.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using WebDemo.Core.Models.Mail;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using WebApiDemo.Core.Models;
using System.IO.Compression;
using WebApiDemo.Dtos;

// SOURCE: https://blog.christian-schou.dk/send-emails-with-asp-net-core-with-mailkit/

namespace WebDemo.Api.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly IUserService _userService;
        private readonly IDistributedCache _cache;
        public MailService(IOptions<MailSettings> settings, IUserService userService, IDistributedCache cache)
        {
            _settings = settings.Value;
            _userService = userService;
            _cache = cache;
        }

        public async Task<bool> SendAsync(MailData mailData, CancellationToken ct = default)
        {
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));
                mail.Sender = new MailboxAddress(mailData.DisplayName ?? _settings.DisplayName, mailData.From ?? _settings.From);

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                // Check if we got any attachments and add the to the builder for our message
                if (mailData.Attachments != null)
                {
                    byte[] attachmentFileByteArray;

                    foreach (IFormFile attachment in mailData.Attachments)
                    {
                        // Check if length of the file in bytes is larger than 0
                        if (attachment.Length > 0)
                        {
                            // Create a new memory stream and attach attachment to mail body
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                // Copy the attachment to the stream
                                attachment.CopyTo(memoryStream);
                                attachmentFileByteArray = memoryStream.ToArray();
                            }
                            // Add the attachment from the byte array
                            body.Attachments.Add(attachment.FileName, attachmentFileByteArray, ContentType.Parse(attachment.ContentType));
                        }
                    }
                }
                #endregion

                #region Send Mail

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

/*                if (_settings.UseSSL)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_settings.UseStartTls)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                }*/
                
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None, ct);

                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                #endregion

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendWithAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct = default)
        {
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                // Check if we got any attachments and add the to the builder for our message
                if (mailData.Attachments != null)
                {
                    foreach (IFormFile attachment in mailData.Attachments)
                    {
                        // Check if length of the file in bytes is larger than 0
                        if (attachment.Length > 0)
                        {
                            // Create a new memory stream and attach attachment to mail body
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                // Copy the attachment to the stream
                                attachment.CopyTo(memoryStream);
                                byte[] attachmentFileByteArray = memoryStream.ToArray();

                                // Add the attachment from the byte array
                                body.Attachments.Add(attachment.FileName, attachmentFileByteArray, ContentType.Parse(attachment.ContentType));
                            }
                        }
                    }
                }

                // Set the body builder to the mail message
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None, ct);

                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                return true;
                #endregion

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendLinkAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct = default)
        {
            try
            {
                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                #region Sender / Receiver
                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, mailData.From ?? _settings.From));

                // Receiver
                foreach (string mailAddress in mailData.To)
                    mail.To.Add(MailboxAddress.Parse(mailAddress));

                // Set Reply to if specified in mail data
                if (!string.IsNullOrEmpty(mailData.ReplyTo))
                    mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

                // BCC
                // Check if a BCC was supplied in the request
                if (mailData.Bcc != null)
                {
                    // Get only addresses where value is not null or with whitespace. x = value of address
                    foreach (string mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }

                // CC
                // Check if a CC address was supplied in the request
                if (mailData.Cc != null)
                {
                    foreach (string mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                        mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));
                }
                #endregion

                #region Content

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = mailData.Subject;
                body.HtmlBody = mailData.Body;
                mail.Body = body.ToMessageBody();

                // Check if we got any attachments and generate links to download them
                if (mailData.Attachments != null)
                {
                    foreach (IFormFile attachment in mailData.Attachments)
                    {
                         // Check if the length of the file in bytes is larger than 0
                if (attachment.Length > 0)
                {
                    // Create a new unique filename for the attachment
                    string uniqueFileName = $"{Guid.NewGuid().ToString()}_{attachment.FileName}";
                    string baseUri = "https://localhost:7271/downloads";

                    // Store the attachment in the distributed cache using the unique filename as key
                    byte[] attachmentFileBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        await attachment.CopyToAsync(memoryStream);
                        attachmentFileBytes = memoryStream.ToArray();
                    }
                    await _cache.SetAsync(uniqueFileName, attachmentFileBytes);

                    // Add a download link for the attachment to the email body
                    body.HtmlBody += $"<p><a href=\"{baseUri}/{uniqueFileName}\">{attachment.FileName}</a></p>";

                    // Add the attachment to the builder for our message
                    body.Attachments.Add(uniqueFileName, attachmentFileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
        }


                // Set the body builder to the mail message
                mail.Body = body.ToMessageBody();

                #endregion

                #region Send Mail

                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                if (_settings.UseSSL)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect, ct);
                }
                else if (_settings.UseStartTls)
                {
                    await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
                }

                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                return true;
                #endregion

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendUserDataAsJsonZipAttachment(int userId, string emailAddress, CancellationToken ct = default)
        {
            try
            {
                // Retrieve user data in JSON format
                var userData = await _userService.GetUserJsonAsync(userId);

                // Compress the JSON data into a ZIP file
                var zipStream = new MemoryStream();
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    var entry = archive.CreateEntry($"user_{userId}.json", CompressionLevel.Fastest);
                    using (var entryStream = entry.Open())
                    using (var writer = new StreamWriter(entryStream))
                    {
                        await writer.WriteAsync(userData);
                    }
                }

                // Initialize a new instance of the MimeKit.MimeMessage class
                var mail = new MimeMessage();

                // Sender
                mail.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));

                // Receiver
                mail.To.Add(MailboxAddress.Parse(emailAddress));

                // Add Content to Mime Message
                var body = new BodyBuilder();
                mail.Subject = $"User data for user {userId}";
                body.HtmlBody = $"<p>Hi,</p><p>Please find attached the user data for user {userId}.</p><p>Best regards,</p>";
                mail.Body = body.ToMessageBody();

                // Add the ZIP file as an attachment to the message
                var attachment = new MemoryStream(zipStream.ToArray());
                body.Attachments.Add($"user_{userId}.zip", attachment);

                // Set the body builder to the mail message
                mail.Body = body.ToMessageBody();

                // Send Mail
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None, ct);
                await smtp.AuthenticateAsync(_settings.UserName, _settings.Password, ct);
                await smtp.SendAsync(mail, ct);
                await smtp.DisconnectAsync(true, ct);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    
    }
}
