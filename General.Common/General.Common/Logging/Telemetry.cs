using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;

namespace General.Common.Logging
{
	public class TelemetryManager : ILogger
	{
		public TelemetryClient Client { get; set; }

		public TelemetryManager(IConfiguration configuration)
		{
			var config = TelemetryConfiguration.CreateDefault();
			config.InstrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
			Client = new TelemetryClient(config);
		}
	}
}
