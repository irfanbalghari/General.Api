using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using General.Cars.Core.Entities;
using General.Common.Logging;
using General.Core.Application.Dto.AppUser;
using General.Core.Application.Dto.Token;
using General.Core.Application.Interfaces.AppUser;
using General.Core.Application.Interfaces.Auth;
using General.Core.Application.Wrappers;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace General.Core.Application.Services.AppUser
{
	public class AuthService : IAuthService
	{
		private readonly IAccountService accountService;
		private readonly UserManager<ApplicationUser> userManager;

		private readonly ITokenService tokenService;
		private readonly IConfiguration configuration;
		private readonly ILogger logger;
		private readonly IUserRepository userRepository;
		private readonly RowEntityContext _dbContext;

		public AuthService(RowEntityContext dbContext, UserManager<ApplicationUser> userManager, IAccountService accountService, ITokenService tokenService, IConfiguration configuration, ILogger logger, IUserRepository userRepository)
		{
			this.userManager = userManager;
			this.accountService = accountService;
			this.configuration = configuration;
			this.tokenService = tokenService;
			this.logger = logger;
			this.userRepository = userRepository;
			this._dbContext = dbContext;
		}

		public async Task<Response<JWTDto>> LoginAsync(LoginDto loginDto)
		{
			Response<JWTDto> response = new Response<JWTDto>() { Success = true, Status = System.Net.HttpStatusCode.OK };
			try
			{
				var user = await accountService.FindUserAsync(new FindUserDto { Email = loginDto.Email });
				if (user.Data == null)
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserNotExists;
					response.Status = System.Net.HttpStatusCode.Unauthorized;
					return response;
				}
				var userProfiles = await userRepository.GetAllAsync();
				if (userProfiles.IsNullOrEmpty())
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserNotExists;
					response.Status = System.Net.HttpStatusCode.Unauthorized;
					return response;
				}
				var appUser = user.Data;
				var userProfile = userProfiles.FirstOrDefault(userProfile => userProfile.AppUserId == appUser.Id);
				if (user.Data != null)
				{
					bool isValidUser = await userManager.CheckPasswordAsync(appUser, loginDto.Password);

					if (isValidUser)
					{
						var userRoles = await userManager.GetRolesAsync(appUser);

						var res = from roleIds in _dbContext.UserRoles.Where(x => appUser.Id == x.UserId).Select(x => x.RoleId)
								  from pr in _dbContext.Permissions
								  from rp in _dbContext.RolePermissions.Where(x => roleIds.Contains(x.RoleId)).Distinct()
								  where pr.Id == rp.PermissionId
								  select pr.PermissionName;

						var userPermissions = res.ToList();

						var authClaims = new List<Claim>
						{
							new Claim("Email", appUser.Email),
							new Claim("Name", userProfile?.FirstName + " " + userProfile?.LastName),
							new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
							new Claim("Id", appUser.Id),
						};

						foreach (var userRole in userRoles)
						{
							authClaims.Add(new Claim("Role", userRole));
						}
						foreach (var permission in userPermissions)
						{
							authClaims.Add(new Claim("Permission", permission));
						}

						response.Data = tokenService.GenerateToken(authClaims).Data;

					}
					else
					{
						response.Success = false;
						response.Message = AppConstants.UserManagement.IncorrectPassword;
						response.Status = System.Net.HttpStatusCode.Unauthorized;
					}

				}
				else
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.InvalidUserName;
					response.Status = System.Net.HttpStatusCode.Unauthorized;
				}
			}
			catch (Exception ex)
			{

				logger.Client.TrackException(ex);
				response.Errors.Add(ex.Message);
				response.Status = System.Net.HttpStatusCode.InternalServerError;
				response.Success = false;
			}
			return response;
		}
	}
}
