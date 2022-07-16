using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using General.Cars.Infrastructure.EFCore.Repositories;
using General.Core.Application.Feature.Attachments.Interface;
using General.Core.Application.Feature.Attachments.Service;
using General.Core.Application.Feature.Communication.Interface;
using General.Core.Application.Feature.Communication.Service;
using General.Core.Application.Feature.Interface;
using General.Core.Application.Feature.Service;
using General.Core.Application.Feature.UserManagement.Interface;
using General.Core.Application.Feature.UserManagement.Service;
using General.Core.Application.Interfaces;
using General.Core.Application.Interfaces.AppUser;
using General.Core.Application.Interfaces.Auth;
using General.Core.Application.Services;
using General.Core.Application.Services.AppUser;
using General.Core.Application.Services.Auth;
using General.Core.Repositories;
using General.Infrastructure.APIGateway.Client.Repositories.LeaseServiceRepos;
using General.Infrastructure.EFCore.Repositories;

namespace General.Core.Application
{
	public static class RegisterServices
	{
		public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddTransient<IUserProfile, UserProfileService>();
			services.AddTransient<ICommunication, CommunicationService>();
			services.AddTransient<IAccountService, AccountService>();
			services.AddTransient<IAuthService, AuthService>();
			services.AddTransient<ITokenService, TokenService>();

			services.AddTransient<IUserManagementService, UserManagementService>();
			services.AddTransient<IAttachmentService, AttachmentService>();
			services.AddTransient<ICommentService, CommentService>();
			RegisterRepositories(services, configuration);
		}

		public static void RegisterRepositories(IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IAttachmentRepository, AttachmentRepository>();
			services.AddScoped<ICommentRepository, CommentRepository>();
			services.AddScoped<IUserRepository, UserProfileRepository>();


		}
	}
}
