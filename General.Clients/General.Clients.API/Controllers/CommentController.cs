using Microsoft.AspNetCore.Mvc;
using General.Core.Application.Feature.DTO;
using General.Core.Application.Feature.Interface;
using General.Core.Application.Feature.Model;
using General.Core.Application.Wrappers;
using System.Threading.Tasks;

namespace General.Clients.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CommentController : ControllerBase
	{
		ICommentService _commentService;
		public CommentController(ICommentService commentService)
		{
			_commentService = commentService;
		}
		[HttpPost]
		public async Task<Response<bool>> CreateComment([FromBody] CommentModel model)
		{
			var result = await _commentService.CreateAsync(model);
			Response.StatusCode = (int)result.Status;
			return result;
		}
		[HttpGet]
		[Route("{id}")]
		public async Task<Response<CommentListDto>> GetCommentsAsync(long id)
		{
			var result = await _commentService.GetCommentsAsync(id);
			Response.StatusCode = (int)result.Status;
			return result;
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<Response<bool>> DeleteCommentAsync(long id)
		{
			var result = await _commentService.DeleteComment(id);
			Response.StatusCode = (int)result.Status;
			return result;
		}
	}
}
