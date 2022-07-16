using General.Common.EmailSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Common.EmailSender
{
    public interface IEmailSender
    {
        Task<bool> Send(EmailModel emailModel);
    }
}
