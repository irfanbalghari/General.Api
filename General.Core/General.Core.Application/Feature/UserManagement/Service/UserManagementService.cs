
using AutoMapper;
using General.Cars.Core.Entities;
using General.Common.Constants;
using General.Common.EmailSender.Models;
using General.Common.Logging;
using General.Common.Password;
using General.Core.Application.Dto;
using General.Core.Application.Dto.AppUser;
using General.Core.Application.Feature.Communication.Interface;
using General.Core.Application.Feature.UserManagement.DTO;
using General.Core.Application.Feature.UserManagement.DTO.ConfirmEmail;
using General.Core.Application.Feature.UserManagement.Interface;
using General.Core.Application.Feature.UserManagement.Model;
using General.Core.Application.Interfaces;
using General.Core.Application.Interfaces.AppUser;
using General.Core.Application.Wrappers;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.UserManagement.Service
{
	public class UserManagementService : IUserManagementService
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IAccountService accountService;
		private readonly GeneralEntityContext dbContext;
		private readonly ICommunication communicationService;
		private readonly Password _password;
		private IHostingEnvironment env;
		private readonly string emailFolderPath = "DefaultEmailTemplates/";
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IUserRepository userRepository;
		ILogger logger;
		IMapper mapper;
		Microsoft.Extensions.Configuration.IConfiguration config;
		private IUserProfile profile;

		public UserManagementService(GeneralEntityContext dbContext, IAccountService accountService, UserManager<ApplicationUser> userManager,
			ICommunication communicationService, IOptions<Password> password, IHostingEnvironment env, SignInManager<ApplicationUser> signInManager,
			 IUserRepository userRepository, ILogger logger, IMapper mapper, Microsoft.Extensions.Configuration.IConfiguration config, IUserProfile profile)
		{
			this.dbContext = dbContext;
			this.accountService = accountService;
			this.userManager = userManager;
			this.communicationService = communicationService;
			this.env = env;
			_password = password.Value;
			this.signInManager = signInManager;
			this.userRepository = userRepository;
			this.logger = logger;
			this.mapper = mapper;
			this.config = config;
			this.profile = profile;
		}

		public async Task<bool> GeneratePasswordResetLink(string userEmail)
		{
			// check whether the employee's account exists or not
			var user = await userManager.FindByEmailAsync(userEmail.ToLower());

			if (user == null)
			{
				return false;
			}
			else
			{
				// get reset link
				var base64Link = await GetResetPasswordTokenInBase64Async(user);
				var resetPasswordLink = config["App:AppUrl"] + config["App:AppResetPassword"] + user.Email + "/" + base64Link;

				// pick email template and populate it
				var folderPath = emailFolderPath + EmailTemplateConstants.ResetPasswordLink + ".html";
				var emailTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(env.WebRootPath, folderPath));
				var userProfile = dbContext.UserProfile.Include(user => user.AppUser).Where(user => user.AppUser.Email == userEmail.ToLower()).FirstOrDefault();
				var emailfixed = communicationService.InsertData(emailTemplate, resetPasswordLink, userProfile, "", null);


				EmailModel emailModel = new EmailModel()
				{
					ToEmail = user.Email,
					Subject = "Reset Password",
					HTMLcontent = emailfixed,
				};

				await communicationService.SendEmail(emailModel);
				return true;
			}
		}

		public async Task<Response<UserProfileUpdatedDto>> RegisterNewUserAsync(DTO.UserProfileDto model)
		{
			Response<UserProfileUpdatedDto> response = new Response<UserProfileUpdatedDto>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = Core.AppConstants.Success
			};
			try
			{
				model.Roles = new RolesDto { Roles = model.RoleList };
				var existingUser = dbContext.UserProfile.Include(user => user.AppUser)
					.Where(user => user.AppUser.Email.ToLower() == model.Email.ToLower())
					.FirstOrDefault();
				if (existingUser == null)
				{
					// add to identity
					ApplicationUser userNew = new ApplicationUser()
					{
						Email = model.Email,
						SecurityStamp = Guid.NewGuid().ToString(),
						UserName = model.Email,
						NormalizedEmail = model.Email,
						PhoneNumber = model.PhoneNumber
					};
					// add identity user
					//string dummyPassword = CreatePassword(8);
					string dummyPassword = "Password@12";
					var result = await userManager.CreateAsync(userNew, dummyPassword);

					// add to profile table
					Entities.UserProfile user = new Entities.UserProfile()
					{
						FirstName = model.FirstName,
						LastName = model.LastName,
						OfficeAddress = model.OfficeAddress,
						AppUser = userNew,
						CreatedOn = DateTime.UtcNow,
						ModifiedOn = DateTime.UtcNow,
						//CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier)
					};
					dbContext.UserProfile.Add(user);

					// add to roles
					await accountService.AddUserToRolesAsync(new FindUserDto
					{
						Email = userNew.Email
					}, model.Roles);

					// send email containing confirmation link with path to set a password
					await GenerateEmailConfirmationLinkAsync(user);

					await dbContext.SaveChangesAsync();
					UserProfileUpdatedDto newUser = new UserProfileUpdatedDto()
					{
						Id = user.Id,
						CreatedBy = user.CreatedBy,
						ModifiedBy = user.ModifiedBy,
						ModifiedOn = user.ModifiedOn,
						Email = user.AppUser.Email,
						FirstName = user.FirstName,
						LastName = user.LastName,
						PhoneNumber = user.AppUser.PhoneNumber,
						Roles = model.Roles,
						OfficeAddress = user.OfficeAddress
					};
					response.Data = newUser;
				}
				else
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserExists;
					response.Status = System.Net.HttpStatusCode.Conflict;
				}
			}
			catch (Exception ex)
			{
				//logger.Client.TrackException(ex);
				response.Errors.Add(ex.Message);
				response.Success = false;
			}
			return response;
		}

		public async Task<Response<UserProfileUpdatedDto>> UpdateUserAsync(int id, DTO.UserProfileDto dto)
		{
			Response<UserProfileUpdatedDto> response = new Response<UserProfileUpdatedDto>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = Core.AppConstants.Success
			};
			try
			{
				dto.Roles = new RolesDto { Roles = dto.RoleList };
				//var existingUser = dbContext.UserProfile.Include(user => user.AppUser).Where(user => user.AppUser.Email.ToLower() == dto.Email.ToLower()).FirstOrDefault();
				var existingUser = userRepository.GetAllQueryable().Where(user => user.Id == id).Include(user => user.AppUser).FirstOrDefault();
				if (existingUser != null)
				{
					// Basics cannot be changed e.g. password and email


					// add to profile table
					existingUser.FirstName = dto.FirstName;
					existingUser.LastName = dto.LastName;
					existingUser.ModifiedOn = DateTime.UtcNow;
					existingUser.OfficeAddress = dto.OfficeAddress;
					existingUser.AppUser.PhoneNumber = dto.PhoneNumber;
					await userRepository.UpdateAsync(existingUser);
					// add to roles
					await accountService.UpdateUserToRolesAsync(new FindUserDto
					{
						Email = dto.Email
					}, dto.Roles);

					UserProfileUpdatedDto newUser = new UserProfileUpdatedDto()
					{
						Id = existingUser.Id,
						CreatedBy = existingUser.CreatedBy,
						CreatedOn = existingUser.CreatedOn,
						ModifiedBy = existingUser.ModifiedBy,
						ModifiedOn = existingUser.ModifiedOn,
						Email = existingUser.AppUser.Email,
						FirstName = existingUser.FirstName,
						LastName = existingUser.LastName,
						PhoneNumber = existingUser.AppUser.PhoneNumber,
						Roles = dto.Roles,
						OfficeAddress = existingUser.OfficeAddress
					};
					response.Data = newUser;
				}
				else
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserNotExists;
				}
			}
			catch (Exception ex)
			{
				//logger.Client.TrackException(ex);
				response.Errors.Add(ex.Message);
				response.Success = false;
			}
			return response;
		}

		public async Task<Response<bool>> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
		{
			Response<bool> response = new Response<bool>() { Success = true };
			try
			{
				var appUser = await userManager.FindByEmailAsync(confirmEmailDto.userEmail);
				//var user = dbContext.UserProfile.Include(user => user.AppUser).Where(user => user.Id == confirmEmailDto.userId).FirstOrDefault();
				if (appUser == null)
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserNotExists;
					return response;
				}
				// convert code from base64 to normal encoded string
				var plainCode = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(confirmEmailDto.emailToken));
				var confirmedIdentityUser = await userManager.ConfirmEmailAsync(appUser, plainCode);
				if (confirmedIdentityUser.Succeeded == false)
				{
					response.Success = false;
					response.Message = "Unable to confirm email. Please recheck email confirmation token";
					return response;
				}

				// get reset link
				var resetToken = await GetResetPasswordTokenInBase64Async(appUser);
				response.Message = resetToken;
			}
			catch (Exception ex)
			{
				//logger.Client.TrackException(ex);
				response.Errors.Add(ex.Message);
				response.Success = false;
			}
			return response;

		}

		public async Task<Response<bool>> ResetPassword(ResetPasswordDto resetPasswordDto)
		{
			Response<bool> response = new Response<bool>() { Success = true };
			try
			{
				var user = await userManager.FindByEmailAsync(resetPasswordDto.userEmail);
				if (user == null)
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserNotExists;
					return response;
				}

				// convert code from base64 to normal encoded string
				var plainCode = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(resetPasswordDto.resetToken));
				var resetPasswordResult = await userManager.ResetPasswordAsync(user, plainCode, resetPasswordDto.newPassword);
				if (resetPasswordResult.Succeeded == false)
				{
					response.Success = false;
					response.Errors = new List<string> { "Unable to reset password. Token Invalid" };
					response.Message = "Unable to reset password. Please recheck reset token";
					return response;
				}

				response.Message = "Password reset successfully";
			}
			catch (Exception ex)
			{
				//logger.Client.TrackException(ex);
				response.Errors.Add(ex.Message);
				response.Success = false;
			}
			return response;

		}
		public async Task<Response<bool>> ChangePassword(ChangePasswordDto passwordDto)
		{

			Response<bool> response = new Response<bool>() { Success = true };
			try
			{
				UserProfile userInfo = profile.GetUserProfile();
				var user = await userManager.FindByEmailAsync(userInfo.Email);
				if (user == null)
				{
					response.Success = false;
					response.Message = AppConstants.UserManagement.UserNotExists;
					return response;
				}

				// convert code from base64 to normal encoded string

				var resetPasswordResult = await userManager.ChangePasswordAsync(user, passwordDto.OldPassword, passwordDto.NewPassword);
				if (resetPasswordResult.Succeeded == false)
				{
					response.Success = false;
					foreach (var item in resetPasswordResult.Errors)
					{
						response.Errors.Add(item.Description);
					}

					response.Message = "Unable to change password.";
					return response;
				}
				response.Data = true;
				response.Success = true;
				response.Message = "Password reset successfully";
			}
			catch (Exception ex)
			{
				//logger.Client.TrackException(ex);
				response.Errors.Add(ex.Message);
				response.Success = false;
			}
			return response;
		}
		private async Task<bool> GenerateEmailConfirmationLinkAsync(Entities.UserProfile userProfile, string defaultPassword = "")
		{
			if (userProfile == null)
			{
				return false;
			}
			else
			{
				// generate reset password token
				var confirmationCode = await userManager.GenerateEmailConfirmationTokenAsync(userProfile.AppUser);
				var base64Link = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(confirmationCode));
				var role = await userManager.GetRolesAsync(userProfile.AppUser);
				var emailConfirmationLink = config["App:AppUrl"] + config["App:AppConfirmEmail"] + userProfile.AppUser.Email + "/" + base64Link;

				// pick email template and populate it
				var folderPath = emailFolderPath + EmailTemplateConstants.EmailConfirmationHtmlTemplate + ".html";
				var emailTemplate = System.IO.File.ReadAllText(System.IO.Path.Combine(env.WebRootPath, folderPath));
				var emailfixed = communicationService.InsertData(emailTemplate, emailConfirmationLink, userProfile, defaultPassword, role);


				EmailModel emailModel = new EmailModel()
				{
					ToEmail = userProfile.AppUser.Email,
					Subject = "Confirm Email",
					HTMLcontent = emailfixed,
				};

				await communicationService.SendEmail(emailModel);
				return true;
			}
		}

		private string CreatePassword(int length)
		{
			const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			StringBuilder res = new StringBuilder();
			Random rnd = new Random();
			while (0 < length--)
			{
				res.Append(valid[rnd.Next(valid.Length)]);
			}

			string DummyPassword = String.Concat(res.ToString(), "$8Ab");
			return DummyPassword;
		}

		public async Task<Response<UserListDto>> GetUserList(UserSearchModel model)
		{
			Response<UserListDto> response = new Response<UserListDto>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = Core.AppConstants.Success
			};

			try
			{
				response.Data = new UserListDto();
				var sortBy = $"@SortBy = NULL,";
				var search = $"@Search = NULL,";
				var roleIds = $"@RoleIds = NULL";
				if (!string.IsNullOrEmpty(model.SortBy) && !string.IsNullOrEmpty(model.SortDirection))
				{
					sortBy = $"@SortBy = '{model.SortBy + " " + model.SortDirection}',";
				}
				if (model.RoleIds?.Count > 0)
				{
					roleIds = $"@RoleIds = '{ListToCommaSeperated(model.RoleIds)}'";
				}
				if (!string.IsNullOrEmpty(model.SearchKeyword))
				{
					search = $"@Search = '{model.SearchKeyword}',";
				}

				string script = $"EXEC SP_USERLIST {sortBy} {search} @StartRowIndex={model.StartRowIndex}, @MaximumRows={model.MaxRows} , {roleIds}";
				var data = dbContext.SPUserList.FromSqlRaw(script).ToList();
				if (data.Count > 0)
				{
					response.Data.TotalCount = data.FirstOrDefault().TotalCount;
				}
				var result = mapper.Map<List<ListUserDto>>(data);
				response.Data.Items = result;
			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Message = Core.AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}

			return response;

		}

		public Response<DTO.GetUser.UserProfileDto> GetUserById(int id)
		{
			Response<DTO.GetUser.UserProfileDto> response = new Response<DTO.GetUser.UserProfileDto>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = Core.AppConstants.Success
			};

			try
			{

				response.Data = new DTO.GetUser.UserProfileDto();
				var user = dbContext.UserProfile.Where(user => user.Id == id).Include(appProfile => appProfile.AppUser).FirstOrDefault();

				if (user != null)
				{
					response.Data = mapper.Map<DTO.GetUser.UserProfileDto>(user);
				}
				else
				{
					response.Success = false;
					response.Status = System.Net.HttpStatusCode.NotFound;
					response.Message = Core.AppConstants.Failed;
				}
			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Message = Core.AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}
			return response;
		}

		public Response<List<RoleDto>> GetRoleList()
		{
			Response<List<RoleDto>> response = new Response<List<RoleDto>>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = Core.AppConstants.Success
			};
			try
			{
				var roles = dbContext.Roles.ToList();
				response.Data = mapper.Map<List<RoleDto>>(roles);

			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Message = Core.AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}
			return response;
		}

		private string ListToCommaSeperated(List<string> lst)
		{
			string result = string.Empty;
			if (lst.Count > 0)
			{
				foreach (var item in lst)
				{
					result += item + ",";
				}
				result = result.Remove(result.LastIndexOf(","));
			}
			else { result = null; }
			return result;
		}

		private async Task<string> GetResetPasswordTokenInBase64Async(ApplicationUser user)
		{
			// generate reset password token
			var resetCode = await userManager.GeneratePasswordResetTokenAsync(user);
			var base64Link = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(resetCode));

			return base64Link;
		}
	}
}
