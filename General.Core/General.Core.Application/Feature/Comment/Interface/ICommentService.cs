using General.Core.Application.Feature.DTO;
using General.Core.Application.Feature.Model;
using General.Core.Application.Wrappers;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.Interface
{
	public interface ICommentService
	{
		Task<Response<bool>> CreateAsync(CommentModel model);
		Task<Response<bool>> DeleteComment(long Id);
		Task<Response<CommentListDto>> GetCommentsAsync(long leaseItemId);
	}
}
