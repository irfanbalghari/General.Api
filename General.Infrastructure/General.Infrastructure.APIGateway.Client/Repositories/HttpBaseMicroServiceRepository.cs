using General.Core.Repositories;
using General.Infrastructure.APIGateway.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.IO;
using System.Threading.Tasks;

namespace General.User.Infrastructure.APIGateway.Client.Repositories
{
    public class HttpBaseMicroServiceRepository : IHttpBaseRepository
	{
        IConfiguration config;
        private readonly RestClient client;
        private string microService;

        public HttpBaseMicroServiceRepository(IConfiguration config, string microService)
        {
            this.config = config;
            this.microService = microService;
            client = new RestClient(config["APIGateway:" + microService + ":Url"]);
        }

        public async Task<IRestResponse> PutData(string url, object data)
        {
            var request = new RestRequest(url, Method.PUT);
            request.AddDefaultHeaders(config, microService);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync(request);
            return response;
        }

        public async Task<IRestResponse> PostData(string url, object data)
        {
            var request = new RestRequest(url, Method.POST);
            request.AddDefaultHeaders(config, microService);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync(request);
            return response;
        }

        public async Task<IRestResponse> GetData(string url)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddDefaultHeaders(config, microService);
            var response = await client.ExecuteAsync(request);
            return response;
        }

        public async Task<IRestResponse> PostFile(string url, IFormFile file)
        {
            var request = new RestRequest(url, Method.POST);
            request.AddDefaultHeaders(config);
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }
            request.AddFile("Files", fileBytes, file.FileName);
            var response = await client.ExecuteAsync(request);
            return response;
        }
    }
}
