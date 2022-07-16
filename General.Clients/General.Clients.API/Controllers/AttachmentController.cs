using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using General.Core.Application.Feature.Attachments.DTO;
using General.Core.Application.Feature.Attachments.Interface;
using General.Core.Application.Wrappers;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace General.Clients.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class AttachmentController : ControllerBase
	{
		IAttachmentService fileUpload;

		public AttachmentController(IAttachmentService fileUpload)
		{
			this.fileUpload = fileUpload;
		}



		[HttpPost]
		[Route("UplaodFile")]
		public async Task<Response<List<AttachmentDto>>> UplaodFileFromDataAsync(string target = "")
		{


			var result = await fileUpload.UploadAsync(Request.Form.Files[0], target);
			Response.StatusCode = (int)result.Status;
			return result;
		}

		[HttpDelete]
		[Route("DeleteFile/{fileType}/fileName")]
		public async Task<Response<bool>> DeleteFileAsync(string fileType, string fileName, CancellationToken cancellationToken)
		{
			var result = await fileUpload.DeleteFileAsync(fileName, fileType);
			Response.StatusCode = (int)result.Status;
			return result;
		}

		[HttpGet, Route("read")]
		public FileContentResult GetDocument(string url)

		{
			byte[] doc;
			using (var webClient = new WebClient())
			{
				doc = webClient.DownloadData(url);


			}
			string mimeType = "application/octet-stream";
			return File(doc, mimeType, "fn.pdf");

		}
		[HttpPost, Route("read-bytes")]
		public FileContentResult GetDocument([FromBody] byte[] doc)

		{
			string mimeType = "application/octet-stream";
			return File(doc, mimeType, "contract.pdf");

		}

	}
}
