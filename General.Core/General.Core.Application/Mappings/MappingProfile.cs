using AutoMapper;
using Microsoft.AspNetCore.Identity;
using General.Core.Application.Feature.Attachments.DTO;
using General.Core.Application.Feature.DTO;
using General.Core.Application.Feature.Model;
using General.Core.Application.Feature.UserManagement.DTO;
using General.Core.Application.Wrappers;
using General.Core.Entities;
using General.Core.Entities.UserManagement;
using System.Collections.Generic;

namespace General.Core.Application.MappingProfiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			MapAttachments();
			MapUsers();
			MapComments();

		}
		void MapAttachments()
		{
			CreateMap<Attachment, AttachmentDto>();
			CreateMap<RestResponse<List<Attachment>>, Response<List<AttachmentDto>>>();
			CreateMap<RestResponse<bool>, Response<bool>>();
		}
		void MapUsers()
		{
			CreateMap<SPUserList, ListUserDto>()
				.ForMember(dest => dest.Roles, src => src.MapFrom(x => MapRoles(x.RoleNames)));
			CreateMap<IdentityRole, RoleDto>();
			CreateMap<UserProfile, General.Core.Application.Feature.UserManagement.DTO.UserProfileDto>()
				.ForMember(dest => dest.Email, src => src.MapFrom(x => x.AppUser.Email))
				.ForMember(dest => dest.PhoneNumber, src => src.MapFrom(x => x.AppUser.PhoneNumber));
		}

		void MapComments()
		{
			CreateMap<CommentModel, Comment>();
			CreateMap<Comment, CommentDto>();
		}
		string[] MapRoles(string commaSeperatedString)
		{
			if (commaSeperatedString != null)
			{
				return commaSeperatedString.Split(",");
			}
			else return new string[] { };
		}
	}
}
