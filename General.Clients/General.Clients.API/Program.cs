using General.Common.Logging;
using General.Infrastructure.EFCore;
using General.Infrastructure.EFCore.EntityContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace General.Clients.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			MigrateDatabase(host);

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

		static void MigrateDatabase(IHost host)
		{
			var logger = host.Services.GetRequiredService<General.Common.Logging.ILogger>();
			logger.Client.TrackEvent("From Program, running the host now.");


			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var loggerFactory = services.GetRequiredService<ILogger>();
				try
				{
					var dbContext = services.GetRequiredService<GeneralEntityContext>();
					try
					{
						if (dbContext.Database.IsSqlServer())
						{
							dbContext.Database.Migrate();
						}
						SchemaSeeder seeder = new SchemaSeeder();
						seeder.Execute(dbContext);
					}
					catch (Exception ex)
					{
						logger.Client.TrackException(ex);
					}

				}
				catch (Exception ex)
				{
					logger.Client.TrackException(ex);
				}
				finally
				{
					logger.Client.TrackEvent("Database update was successfull");
				}
			}
		}

	}
}
