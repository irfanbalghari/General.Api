using General.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static General.Core.AppConstants;

namespace General.Infrastructure.APIGateway.Client.Repositories
{
    public class RestClientBaseRepository<T, TResponse> : IRestClientBaseRepository<T, TResponse> where T : class where TResponse : class
    {
        IConfiguration config;
        private readonly RestClient client;
        private string microService;
        protected RestClientBaseRepository(IConfiguration config, string microService = MicroServiceConfig.Cars)
        {
            this.config = config;
            this.microService = microService;
            client = new RestClient(config["APIGateway:" + microService + ":Url"]);
        }
        public async Task<T> GetAsync(string url)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddDefaultHeaders(config);
            var response = await client.ExecuteAsync<T>(request);
            return response.Data;
        }
        public async Task<T> Put(string url, T data)
        {
            var request = new RestRequest(url, Method.PUT);
            request.AddDefaultHeaders(config);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync<T>(request);
            return response.Data;
        }
        public async Task<T> Post(string url, T data)
        {
            var request = new RestRequest(url, Method.POST);
            request.AddDefaultHeaders(config);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync<T>(request);
            return response.Data;
        }

        public async Task<TResponse> Post(string url, T data, bool GetResponse)
        {
            var request = new RestRequest(url, Method.POST);
            request.AddDefaultHeaders(config);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync<TResponse>(request);
            return response.Data;
        }
        public async Task<IRestResponse> PostData(string url, T data)
        {
            var request = new RestRequest(url, Method.POST);
            request.AddDefaultHeaders(config);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync(request);
            return response;
        }
        public async Task<bool> Delete(string url)
        {
            var request = new RestRequest(url, Method.DELETE);
            request.AddDefaultHeaders(config);
            var response = await client.ExecuteAsync<T>(request);

            return true;
        }

        public async Task<List<T>> GetlistAsync(string url)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddDefaultHeaders(config);
            var response = await client.ExecuteAsync<List<T>>(request);
            return response.Data;
        }

        public async Task<T> PostFile(string url, IFormFile file)
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
            var response = await client.ExecuteAsync<T>(request);
            return response.Data;
        }

        public async Task<TResponse> GetWarappedResponse(string url)
        {
            var request = new RestRequest(url, Method.GET);
            request.AddDefaultHeaders(config);
            var response = await client.ExecuteAsync<TResponse>(request);
            return response.Data;
        }

        public async Task<TResponse> Put(string url, T data, bool GetResponse)
        {
            var request = new RestRequest(url, Method.PUT);
            request.AddDefaultHeaders(config, microService);
            request.AddJsonBody(data);

            var response = await client.ExecuteAsync<TResponse>(request);
            return response.Data;
        }
    }
}
