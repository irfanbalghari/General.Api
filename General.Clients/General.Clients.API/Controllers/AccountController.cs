using General.Core.Application.Dto.AppUser;
using General.Core.Application.Dto.Token;
using General.Core.Application.Interfaces.AppUser;
using General.Core.Application.Interfaces.Auth;
using General.Core.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace General.Clients.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		IAuthService authService;
		IAccountService accountService;
		IConfiguration config;
		public AccountController(IAuthService authService, IAccountService accountService, IConfiguration config)
		{
			this.authService = authService;
			this.accountService = accountService;
			this.config = config;
		}
		[HttpPost]
		[Route("login")]
		public async Task<Response<JWTDto>> Login([FromBody] LoginDto model)
		{
			var result = await authService.LoginAsync(model);
			Response.StatusCode = (int)result.Status;
			return result;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
		{
			var result = await accountService.RegisterUserAsync(model);
			return Ok(result);
		}

		[HttpGet]
		[Route("GetConnection")]
		[Authorize]
		public IActionResult GetConnection()
		{
			var result = config.GetConnectionString("DefaultConnection");
			return Ok(result);
		}
	}
}
