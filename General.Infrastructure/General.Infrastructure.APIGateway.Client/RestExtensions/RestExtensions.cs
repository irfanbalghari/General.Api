using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static General.Core.AppConstants;

namespace General.Infrastructure.APIGateway.Client
{
    public static class RestExtensions
    {
        public static RestRequest AddDefaultHeaders(this RestRequest request, IConfiguration config, string microService = MicroServiceConfig.Cars) {
            request.AddHeader(config["APIGateway:" + microService + ":AuthKeyName"], config["APIGateway:" + microService + ":AuthKeyValue"]);
            return request;
        }
    }
}
