
namespace General.Core.Entities
{
    public class RestPagedResponse<T> : RestResponse<T>
    {
        public RestPagedResponse(T data, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = data;
            Message = null;
            Success = true;
            Errors = null;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
