using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using General.Cars.Core.Entities;
using General.Common.EmailSender;
using General.Common.Logging;
using General.Core.Application;
using General.Infrastructure.EFCore.EntityContext;
using System.Text;

namespace General.Clients.API
{
	public static class ConfigureServices
	{
		private const string DefaultCorsPolicyName = "localhost";

		public static void ConfigureAppServices(this IServiceCollection services, IConfiguration Configuration)
		{
			services.AddSingleton<IEmailSender, SendGridSender>();
			services.AddSingleton<ILogger, TelemetryManager>();
			services.AddDbContext<RowEntityContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
			services.AddApplication(Configuration);
			services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
			{
				opt.Password.RequiredLength = 8;
				opt.Password.RequireDigit = true;
				opt.Password.RequireUppercase = true;
				opt.User.RequireUniqueEmail = true;
			}).AddEntityFrameworkStores<RowEntityContext>().AddDefaultTokenProviders();
			services.AddControllers();
			AddCORS(services, Configuration);
			AddJWT(services, Configuration);
		}

		static void AddCORS(IServiceCollection services, IConfiguration configuration)
		{
			services.AddCors(
				  options => options.AddPolicy(
					  DefaultCorsPolicyName,
					  builder => builder
						  .WithOrigins(
							  configuration["App:CorsOrigins"].Split(","))
						  .SetIsOriginAllowedToAllowWildcardSubdomains()
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials()
						  .WithExposedHeaders("x-token-expired", "x-tenant-inactive")));
		}
		static void AddJWT(IServiceCollection services, IConfiguration configuration)
		{
			// Adding Authentication  
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})

			//Adding Jwt Bearer
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = configuration["JWT:ValidAudience"],
					ValidIssuer = configuration["JWT:ValidIssuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
				};
			});
		}
	}
}
