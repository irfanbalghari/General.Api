using Microsoft.AspNetCore.Mvc;
using General.Core.Application.Feature.Communication.Interface;
using General.Core.Application.Feature.Communication.Model;
using General.Core.Application.Wrappers;
using System.Threading.Tasks;

namespace General.Clients.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommunicationController : ControllerBase
	{
		ICommunication communication;

		public CommunicationController(ICommunication communication)
		{
			this.communication = communication;
		}

		[HttpPost, Route("contact-us")]

		public async Task<Response<bool>> SendEmailAsync([FromBody] ContactUsModel model)
		{

			var result = await communication.ContactUs(model);
			Response.StatusCode = (int)result.Status;
			return result;
		}

		//[HttpPost, Route("general")]

		//public async Task<Response<bool>> Submit([FromBody] RenewalModel model)
		//{

		//    var result = await communication.SubmitRenewalOffer(model);
		//    Response.StatusCode = (int)result.Status;
		//    return result;
		//}
	}
}
