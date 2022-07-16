using General.Core.Application.Dto.AppUser;
using General.Core.Application.Wrappers;
using General.Cars.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Interfaces.AppUser
{
    public interface IAccountService
    {
        Task<Response<IdentityResult>> RegisterUserAsync(RegisterUserDto dto);
        Task<Response<ApplicationUser>> FindUserAsync(FindUserDto findUserDto);
        Task<Response<bool>> AddUserToRolesAsync(FindUserDto findUserDto, RolesDto rolesDto);
        Task<Response<string>> GenerateEmailConfirmationTokenAsync(FindUserDto findUserDto);
        Task<Response<string>> ConfirmEmailAsync(FindUserDto findUserDto, string token);
        Task<Response<string>> GeneratePasswordResetTokenAsync(FindUserDto findUserDto);
        Task<Response<string>> SendPasswordResetEmail(FindUserDto findUserDto, string token, string newPassword);
        Task<Response<List<ApplicationUser>>> FindUserByIdAsync(List<string> userIds);
        Task<Response<bool>> UpdateUserToRolesAsync(FindUserDto findUserDto, RolesDto rolesDto);
    }
}
