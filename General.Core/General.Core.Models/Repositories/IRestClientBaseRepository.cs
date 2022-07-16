using Microsoft.AspNetCore.Http;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Repositories
{
    public interface IRestClientBaseRepository<T, TResponse>
         where T : class
        where TResponse : class
    {
        Task<T> GetAsync(string url);
        Task<TResponse> GetWarappedResponse(string url);
        Task<List<T>> GetlistAsync(string url);
        Task<T> Put(string url, T data);
        Task<TResponse> Put(string url, T data, bool GetResponse);
        Task<T> Post(string url, T data);
        Task<TResponse> Post(string url, T data, bool GetResponse);
        Task<bool> Delete(string url);
        Task<T> PostFile(string url, IFormFile file);
        Task<IRestResponse> PostData(string url, T data);
    }
}
