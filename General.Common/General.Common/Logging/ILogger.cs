using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Common.Logging
{
    public interface ILogger
    {
        TelemetryClient Client { get; set; }

    }
}
