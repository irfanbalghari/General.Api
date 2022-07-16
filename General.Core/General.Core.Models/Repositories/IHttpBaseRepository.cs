using Microsoft.AspNetCore.Http;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Repositories
{
    public interface IHttpBaseRepository
    {
        Task<IRestResponse> PutData(string url, object data);
        Task<IRestResponse> PostData(string url, object data);
        Task<IRestResponse> GetData(string url);
        Task<IRestResponse> PostFile(string url, IFormFile file);
    }
}
