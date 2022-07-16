using AutoMapper;
using Microsoft.AspNetCore.Http;
using General.Common.Logging;
using General.Core.Application.Feature.Attachments.DTO;
using General.Core.Application.Feature.Attachments.Interface;
using General.Core.Application.Wrappers;
using General.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.Attachments.Service
{
	public class AttachmentService : IAttachmentService
	{
		IAttachmentRepository attachmentRepository;
		ILogger logger;
		IMapper mapper;
		public AttachmentService(IAttachmentRepository attachmentRepository, ILogger logger, IMapper mapper)
		{
			this.attachmentRepository = attachmentRepository;
			this.logger = logger;
			this.mapper = mapper;
		}
		public async Task<Response<List<AttachmentDto>>> UploadAsync(IFormFile file, string target = "")
		{
			Response<List<AttachmentDto>> response = new Response<List<AttachmentDto>>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = General.Core.AppConstants.Success
			};

			try
			{
				if (string.IsNullOrEmpty(target))
				{
					//var restResult = await attachmentRepository.PostFile("api/Attachment/UplaodFile", file);
					//var result = mapper.Map<Response<List<AttachmentDto>>>(restResult);
					//response = result;
				}

			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Message = General.Core.AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}

			return response;
		}

		public async Task<Response<bool>> DeleteFileAsync(string fileName, string filePath)
		{
			Response<bool> response = new Response<bool>() { Status = System.Net.HttpStatusCode.OK, Success = true };
			try
			{
				//var restResult = await attachmentRepository.Delete(string.Format("api/Attachment/DeleteFile/{0}/{1}", filePath, fileName));
				//response.Data = restResult;
			}
			catch (Exception ex)
			{
				logger.Client.TrackException(ex);
				response.Message = General.Core.AppConstants.Failed;
				response.Status = System.Net.HttpStatusCode.BadRequest;
			}
			return response;
		}


	}
}
