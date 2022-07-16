using General.Core.Application.Dto.AppUser;
using General.Core.Application.Feature.UserManagement.Model;
using General.Core.Application.Interfaces.AppUser;
using General.Core.Application.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Cars.Core.Entities;
using General.Common.Logging;

namespace General.Core.Application.Services.AppUser
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
       
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        public AccountService(UserManager<ApplicationUser> userManager,/* RoleManager<IdentityRole> roleManager,*/ IConfiguration configuration, ILogger logger)
        {
            this.userManager = userManager;
            
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<Response<IdentityResult>> RegisterUserAsync(RegisterUserDto dto)
        {
            Response<IdentityResult> response = new Response<IdentityResult>() { Success = true };
            try
            {
                var userExists = await userManager.FindByNameAsync(dto.Email);
                if (userExists != null)
                {
                    response.Success = false;
                    response.Message = AppConstants.UserManagement.UserExists;
                }
                else
                {
                    ApplicationUser user = new ApplicationUser()
                    {
                        Email = dto.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = dto.Email
                    };
                    var result = await userManager.CreateAsync(user, dto.Password);

                    await AddUserToRolesAsync(new FindUserDto
                    {
                        Email = user.Email
                    }, dto.Roles);

                    response.Data = result;
                }
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<ApplicationUser>> FindUserAsync(FindUserDto findUserDto)
        {
            Response<ApplicationUser> response = new Response<ApplicationUser>() { Success = true };
            try
            {
                response.Data = await userManager.FindByEmailAsync(findUserDto.Email);
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<bool>> AddUserToRolesAsync(FindUserDto findUserDto, RolesDto rolesDto)
        {
            Response<bool> response;
            var user = await FindUserAsync(findUserDto);
            if (user.Data != null)
            {
                await userManager.AddToRolesAsync(user.Data, rolesDto.Roles);
                response = new Response<bool>(true, AppConstants.Success);
            }
            else
            {
                response = new Response<bool>(AppConstants.Failed);
            }
            return response;
        }

        public async Task<Response<string>> GenerateEmailConfirmationTokenAsync(FindUserDto findUserDto)
        {
            Response<string> response = new Response<string>() { Success = true };
            try
            {
                var user = await FindUserAsync(findUserDto);
                if (user != null)
                {
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(user.Data);
                    response.Data = token;
                    if (!string.IsNullOrEmpty(user.Data.Email))
                    {
                        //TODO: send email;
                    }

                }
                else
                {
                    response = new Response<string>(AppConstants.UserManagement.UserNotExists);
                }
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<string>> ConfirmEmailAsync(FindUserDto findUserDto, string token)
        {
            Response<string> response = new Response<string>() { Success = true };
            try
            {
                var user = await FindUserAsync(findUserDto);
                if (user != null)
                {
                    await userManager.ConfirmEmailAsync(user.Data, token);
                    response.Data = AppConstants.Success;
                    if (!string.IsNullOrEmpty(user.Data.Email))
                    {
                        //TODO: send email;
                    }
                }
                else
                {
                    response = new Response<string>(AppConstants.UserManagement.UserNotExists);
                }
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<string>> GeneratePasswordResetTokenAsync(FindUserDto findUserDto)
        {
            Response<string> response = new Response<string>() { Success = true };
            try
            {
                var user = await FindUserAsync(findUserDto);
                if (user != null)
                {
                    string token = await userManager.GeneratePasswordResetTokenAsync(user.Data);
                    response.Data = token;
                }
                else
                {
                    response = new Response<string>(AppConstants.UserManagement.UserNotExists);
                }
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<string>> SendPasswordResetEmail(FindUserDto findUserDto, string token, string newPassword)
        {
            Response<string> response = new Response<string>() { Success = true };
            try
            {
                var user = await FindUserAsync(findUserDto);
                if (user != null)
                {
                    await userManager.ResetPasswordAsync(user.Data, token, newPassword);
                    response.Data = AppConstants.Success;
                    if (!string.IsNullOrEmpty(user.Data.Email))
                    {
                        //TODO: send email;
                    }
                }
                else
                {
                    response = new Response<string>(AppConstants.UserManagement.UserNotExists);
                }
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<List<ApplicationUser>>> FindUserByIdAsync(List<string> userId)
        {
            Response<List<ApplicationUser>> response = new Response<List<ApplicationUser>>() { Success = true };
            try
            {
                var user = await userManager.Users.Where(x => userId.Contains(x.Id)).Distinct().ToListAsync();
                response.Data = user;
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }

        public async Task<Response<bool>> UpdateUserToRolesAsync(FindUserDto findUserDto, RolesDto rolesDto)
        {
            Response<bool> response;
            var user = await FindUserAsync(findUserDto);
            if (user.Data != null)
            {
                var oldRoles = await userManager.GetRolesAsync(user.Data);
                await userManager.RemoveFromRolesAsync(user.Data, oldRoles);
                await userManager.AddToRolesAsync(user.Data, rolesDto.Roles);
                response = new Response<bool>(true, AppConstants.Success);
            }
            else
            {
                response = new Response<bool>(AppConstants.Failed);
            }
            return response;
        }



    }
}
