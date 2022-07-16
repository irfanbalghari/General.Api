using General.Core.Application.Feature.Attachments.DTO;
using General.Core.Application.Wrappers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.Attachments.Interface
{
    public interface IAttachmentService
    {
        Task<Response<List<AttachmentDto>>> UploadAsync(IFormFile file, string target = "");
        Task<Response<bool>> DeleteFileAsync(string fileName, string filePath);
    }
}
