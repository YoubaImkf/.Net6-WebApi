using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using WebDemo.Core.Interfaces;
using WebDemo.Core.Models.Mail;

namespace WebDemo.Api.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mail;

        public MailController(IMailService mail)
        {
            _mail = mail;
        }

        [HttpPost("sendmail")]
        public async Task<IActionResult> SendMailAsync(MailData mailData)
        {
            bool result = await _mail.SendAsync(mailData, new CancellationToken());

            if (result)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail has successfully been sent.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail could not be sent.");
            }
        }

        [HttpPost("sendemailwithattachment")]
        public async Task<IActionResult> SendMailWithAttachmentAsync([FromForm] MailDataWithAttachments mailData)
        {
            bool result = await _mail.SendWithAttachmentsAsync(mailData, new CancellationToken());

            if (result)
            {
                return StatusCode(StatusCodes.Status200OK, "Mail with attachment has successfully been sent.");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occured. The Mail with attachment could not be sent.");
            }
        }
    }
}
