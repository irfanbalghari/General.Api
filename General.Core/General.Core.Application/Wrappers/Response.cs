using System;
using System.Collections.Generic;
using System.Net;

namespace General.Core.Application.Wrappers
{
    public class Response<T>
    {
        private List<string> errors = new List<string>();

        public Response()
        {
        }

        public Response(T data, string message = null)
        {
            Success = true;
            Message = message;
            Data = data;
        }

        public Response(string message)
        {
            Success = false;
            Message = message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }
        public HttpStatusCode Status { get; set; }

        public List<string> Errors { get { return errors; } set { errors = value; } }

        public T Data { get; set; }

        public static implicit operator Response<T>(Response<object> v)
        {
            throw new NotImplementedException();
        }
    }
}