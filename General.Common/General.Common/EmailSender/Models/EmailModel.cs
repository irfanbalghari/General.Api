using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Common.EmailSender.Models
{
    public class EmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string PlainText { get; set; }
        public string HTMLcontent { get; set; }
        public List<SendGrid.Helpers.Mail.Attachment> Attachments { get; set; }
    }
}
