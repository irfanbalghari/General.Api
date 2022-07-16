using AutoMapper;
using General.Common.Logging;
using General.Core.Application.Dto;
using General.Core.Application.Feature.DTO;
using General.Core.Application.Feature.Interface;
using General.Core.Application.Feature.Model;
using General.Core.Application.Interfaces;
using General.Core.Application.Interfaces.AppUser;
using General.Core.Application.Wrappers;
using General.Core.Entities;
using General.Core.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.Service
{
	public class CommentService : ICommentService
	{
		private readonly ILogger logger;
		private readonly IMapper mapper;
		private IUserProfile profile;
		private readonly ICommentRepository repo;
		Dto.UserProfile UserProfile;
		IAccountService _accountService;

		public CommentService(IAccountService accountService, ILogger logger, IMapper mapper, ICommentRepository repo, IUserProfile profile)
		{
			this.logger = logger;
			this.mapper = mapper;
			this.repo = repo;
			this.profile = profile;
			this._accountService = accountService;
		}
		public async Task<Response<bool>> CreateAsync(CommentModel model)
		{
			Response<bool> response = new Response<bool>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = AppConstants.Success
			};
			try
			{
				UserProfile = profile.GetUserProfile();
				if (UserProfile.Id != null)
				{
					var item = mapper.Map<Comment>(model);
					item.UserId = UserProfile.Id;
					item.CreatedOn = DateTime.UtcNow;
					var result = await repo.AddAsync(item);
					if (result != null)
					{
						response.Status = System.Net.HttpStatusCode.OK;
					}
					else
					{
						response.Status = System.Net.HttpStatusCode.InternalServerError;
						response.Success = false;
					}
				}
				else
				{
					response.Success = false;
					response.Message = AppConstants.Failed;
					response.Status = System.Net.HttpStatusCode.BadRequest;
				}

			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Success = false;
				response.Message = AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}

			return response;
		}

		public async Task<Response<bool>> DeleteComment(long itemId)
		{
			Response<bool> response = new Response<bool>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = AppConstants.Success
			};
			try
			{
				var cmt = repo.GetAllQueryable().FirstOrDefault(x => x.Id == itemId);
				await repo.DeleteAsync(cmt);
			}

			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Success = false;
				response.Message = AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;

			}

			return response;
		}

		public async Task<Response<CommentListDto>> GetCommentsAsync(long leaseItemId)
		{
			Response<CommentListDto> response = new Response<CommentListDto>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = AppConstants.Success
			};
			try
			{
				response.Data = new CommentListDto();
				var lst = repo.GetAllQueryable();
				lst = lst.Where(x => x.EntityTypeId == leaseItemId && x.Deleted == false).OrderByDescending(x => x.CreatedOn);
				var count = lst.Count();
				var records = lst.ToList();
				response.Data.TotalCount = count;
				var userIds = records.Select(x => x.UserId).Distinct();
				var users = await _accountService.FindUserByIdAsync(userIds.ToList());
				var res = from u in users.Data
						  from r in records
						  where u.Id == r.UserId
						  select new CommentDto
						  {
							  Id = r.Id,
							  UserName = u.UserName,
							  EntityTypeId = r.EntityTypeId,
							  UserId = r.UserId,
							  Description = r.Description,
							  CreatedOn = r.CreatedOn
						  };
				var comments = res.OrderByDescending(x => x.CreatedOn).ToList();
				response.Data.TotalCount = res.Count();
				response.Data.Items = comments;
			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Success = false;
				response.Message = AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}

			return response;
		}

	}
}
