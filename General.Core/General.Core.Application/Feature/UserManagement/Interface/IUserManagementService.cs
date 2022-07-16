
using System.Collections.Generic;
using System.Threading.Tasks;
using General.Core.Application.Feature.UserManagement.DTO;
using General.Core.Application.Feature.UserManagement.DTO.ConfirmEmail;
using General.Core.Application.Feature.UserManagement.DTO.GetUser;
using General.Core.Application.Feature.UserManagement.Model;
using General.Core.Application.Wrappers;
using General.Core.Entities;
using General.Cars.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace General.Core.Application.Feature.UserManagement.Interface
{
    public interface IUserManagementService
    {
        Task<Response<UserProfileUpdatedDto>> RegisterNewUserAsync(DTO.UserProfileDto dto);
        Task<Response<UserProfileUpdatedDto>> UpdateUserAsync(int id, DTO.UserProfileDto dto);
        Task<bool> GeneratePasswordResetLink(string userEmail);
        Task<Response<bool>> ConfirmEmail(ConfirmEmailDto confirmEmailDto);
        Task<Response<bool>> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<Response<UserListDto>> GetUserList(UserSearchModel userSearchModel);
        Response<List<RoleDto>> GetRoleList();
        Response<DTO.GetUser.UserProfileDto> GetUserById(int id);
        Task<Response<bool>> ChangePassword(ChangePasswordDto passwordDto);
    }
}
