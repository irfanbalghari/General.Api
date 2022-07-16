using General.Common.EmailSender.Models;
using General.Core.Application.Feature.Communication.Model;
using General.Core.Entities;
using General.Core.Application.Wrappers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace General.Core.Application.Feature.Communication.Interface
{
    public interface ICommunication
    {

        Task<Response<bool>> ContactUs(ContactUsModel model);
        Task<Response<bool>> SendEmail(EmailModel emailModel);
        Task<Response<bool>> SubmitRenewalOffer(RenewalModel model);
        string InsertData(string emailTemplate, string callbackUrl, UserProfile user,
                                               string dummyPassword, IList<string> roles);
    }
}
