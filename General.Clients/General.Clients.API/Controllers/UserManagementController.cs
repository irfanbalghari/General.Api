using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using General.Core.Application.Feature.UserManagement.DTO;
using General.Core.Application.Feature.UserManagement.DTO.ConfirmEmail;
using General.Core.Application.Feature.UserManagement.Interface;
using General.Core.Application.Feature.UserManagement.Model;
using General.Core.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace General.Clients.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserManagementController : ControllerBase
	{
		IUserManagementService userManagementService;
		public UserManagementController(IUserManagementService userManagementService)
		{
			this.userManagementService = userManagementService;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] General.Core.Application.Feature.UserManagement.DTO.UserProfileDto model)
		{
			var result = await userManagementService.RegisterNewUserAsync(model);
			if (result.Status == System.Net.HttpStatusCode.Conflict)
			{
				return Conflict();
			}
			return Ok(result);
		}

		[HttpPut]
		[Route("update-user/{id}")]
		[Authorize]
		public async Task<IActionResult> Update([FromRoute] int id, [FromBody] General.Core.Application.Feature.UserManagement.DTO.UserProfileDto model)
		{
			var result = await userManagementService.UpdateUserAsync(id, model);
			if (result.Status == System.Net.HttpStatusCode.NotFound)
			{
				return NotFound();
			}
			return Ok(result);
		}

		[HttpPost]
		[Route("search")]
		[Authorize]
		public async Task<Response<UserListDto>> Search([FromBody] UserSearchModel searchModel)
		{
			var result = await userManagementService.GetUserList(searchModel);
			Response.StatusCode = (int)result.Status;
			return result;

		}

		[HttpGet]
		[Route("roles")]
		[Authorize]
		public Response<List<RoleDto>> GetRoleList()
		{
			var result = userManagementService.GetRoleList();
			Response.StatusCode = (int)result.Status;
			return result;

		}

		[AllowAnonymous]
		[HttpPost]
		[Route("forgot-password")]
		public async Task<IActionResult> Generate_Link([FromBody] string userEmail)
		{
			try
			{
				var sendStatus = await userManagementService.GeneratePasswordResetLink(userEmail);

				if (sendStatus == false)
				{
					return StatusCode(StatusCodes.Status304NotModified, "No account found against this email");
				}
				else
				{
					return StatusCode(StatusCodes.Status200OK);
				}
			}
			catch (Exception E)
			{
				return StatusCode(StatusCodes.Status400BadRequest, E.ToString());
			}

		}

		[AllowAnonymous]
		[HttpPost]
		[Route("confirm-email")]
		public async Task<IActionResult> Confirm_Password([FromBody] ConfirmEmailDto confirmEmailDto)
		{
			var response = await userManagementService.ConfirmEmail(confirmEmailDto);
			if (response.Success == false)
			{
				return BadRequest(response);
			}
			return Ok(response);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("reset-password")]
		public async Task<IActionResult> Reset_Password([FromBody] ResetPasswordDto resetPasswordDto)
		{
			var response = await userManagementService.ResetPassword(resetPasswordDto);
			if (response.Success == false)
			{
				return BadRequest(response);
			}
			return Ok(response);
		}
		/// <summary>
		/// Only logged in users can change current password
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Authorize, HttpPost, Route("change-password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
		{
			var response = await userManagementService.ChangePassword(dto);
			if (response.Success == false)
			{
				return BadRequest(response);
			}
			return Ok(response);
		}

		[HttpGet]
		[Route("user/{id}")]
		[Authorize]
		public Response<General.Core.Application.Feature.UserManagement.DTO.GetUser.UserProfileDto> GetUserById(int id)
		{
			var result = userManagementService.GetUserById(id);
			Response.StatusCode = (int)result.Status;
			return result;

		}
	}
}
