using General.Common.EmailSender.Models;
using General.Common.Logging;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Common.EmailSender
{
    public class SendGridSender : IEmailSender
    {
        public IConfiguration config { get; set; }
        string apiKey;
        string fromEmail;
        string fromEmailDisplayName;
        ILogger logger;

        public SendGridSender(IConfiguration config, ILogger logger)
        {
            this.config = config;
            this.logger = logger;
            apiKey = config["EmailGateway:SendGrid:APIKey"];
            fromEmail = config["EmailGateway:SendGrid:FromEmail"];
            fromEmailDisplayName = config["EmailGateway:SendGrid:FromEmailDisplayName"];

        }

        public async Task<bool> Send(EmailModel emailModel)
        {
            try
            {
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(fromEmail, fromEmailDisplayName);

                var subject = emailModel.Subject;
                var to = new EmailAddress(emailModel.ToEmail, emailModel.Subject);
                var plainTextContent = emailModel.PlainText;
                var htmlContent = emailModel.HTMLcontent;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                
                
                if (emailModel.Attachments != null)
                {
                    msg.Attachments = new System.Collections.Generic.List<SendGrid.Helpers.Mail.Attachment>();
                    foreach (var item in emailModel.Attachments)
                    {
                        msg.Attachments.Add(item);
                    }
                }
                
                var response = await client.SendEmailAsync(msg);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                return false;
            }
        }

    }
}
