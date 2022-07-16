
using Microsoft.Extensions.Configuration;
using General.Common.Constants;
using General.Common.EmailSender;
using General.Common.EmailSender.Models;
using General.Common.Logging;
using General.Core.Application.Feature.Communication.Interface;
using General.Core.Application.Feature.Communication.Model;
using General.Core.Application.Wrappers;
using General.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.Communication.Service
{
	public class CommunicationService : ICommunication
	{
		IEmailSender emailSender;
		ILogger logger;
		IConfiguration config;

		public CommunicationService(IEmailSender emailSender, ILogger logger, IConfiguration config)
		{
			this.logger = logger;
			this.emailSender = emailSender;
			this.config = config;
		}

		async Task<Response<bool>> ICommunication.ContactUs(ContactUsModel model)
		{
			Response<bool> response = new Response<bool>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = General.Core.AppConstants.Success
			};

			try
			{
				EmailModel email = new EmailModel();
				email.HTMLcontent = string.Format("<p>{3}</p><div><b>Afsenderoplysninger</b><p>Navn: <b>{0}</b></p><p>E-mail: <a><b>{1}</b></a></p><p>Telefonnummer: <b>{2}</b></p></div>",
					model.Name, model.Email, model.Phone, model.Message);
				email.Subject = "General: contact us";
				email.ToEmail = config["Communication:SystemToEmail"];

				var result = await emailSender.Send(email);
				if (result)
				{
					response.Data = true;
				}
				else
				{
					response.Data = false;
					response.Message = General.Core.AppConstants.Failed;
					response.Status = System.Net.HttpStatusCode.BadRequest;
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

		async Task<Response<bool>> ICommunication.SendEmail(EmailModel emailModel)
		{
			Response<bool> response = new Response<bool>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = General.Core.AppConstants.Success
			};

			try
			{
				// email model already contains details like to, subject, content etc
				var result = await emailSender.Send(emailModel);
				if (result)
				{
					response.Data = true;
				}
				else
				{
					response.Data = false;
					response.Message = General.Core.AppConstants.Failed;
					response.Status = System.Net.HttpStatusCode.BadRequest;
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

		public async Task<Response<bool>> SubmitRenewalOffer(RenewalModel model)
		{
			Response<bool> response = new Response<bool>()
			{
				Success = true,
				Status = System.Net.HttpStatusCode.OK,
				Message = General.Core.AppConstants.Success
			};

			try
			{
				string emailTemplate = System.IO.File.ReadAllText("email-templates/renewal.html");
				if (!string.IsNullOrEmpty(emailTemplate))
				{
					emailTemplate = emailTemplate.Replace("#brand#", !string.IsNullOrEmpty(model.Brand) ? model.Brand : "-");
					emailTemplate = emailTemplate.Replace("#model#", !string.IsNullOrEmpty(model.Model) ? model.Model : "-");
					emailTemplate = emailTemplate.Replace("#licenseplate#", !string.IsNullOrEmpty(model.LicensePlate) ? model.LicensePlate : "-");
					emailTemplate = emailTemplate.Replace("#kilometers#", !string.IsNullOrEmpty(model.Kilometers) ? model.Kilometers : "-");
					emailTemplate = emailTemplate.Replace("#residual#", !string.IsNullOrEmpty(model.ResidualValue) ? model.ResidualValue : "-");
					emailTemplate = emailTemplate.Replace("#name#", !string.IsNullOrEmpty(model.Name) ? model.Name : "-");
					emailTemplate = emailTemplate.Replace("#email#", !string.IsNullOrEmpty(model.Email) ? model.Email : "-");
					emailTemplate = emailTemplate.Replace("#phone#", !string.IsNullOrEmpty(model.Phone) ? model.Phone : "-");
					emailTemplate = emailTemplate.Replace("#year#", !string.IsNullOrEmpty(model.Year) ? model.Year : "-");

					EmailModel email = new EmailModel();
					email.HTMLcontent = emailTemplate;
					email.Subject = "General: details from customer";
					email.ToEmail = config["Communication:SystemToEmail"];

					var result = await emailSender.Send(email);
					if (result)
					{
						response.Data = true;
					}
					else
					{
						response.Data = false;
						response.Message = General.Core.AppConstants.Failed;
						response.Status = System.Net.HttpStatusCode.BadRequest;
					}
				}
				else
				{
					response.Data = false;
					response.Message = General.Core.AppConstants.Failed;
					response.Status = System.Net.HttpStatusCode.BadRequest;
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

		public string InsertData(string emailTemplate, string callbackUrl, UserProfile user,
											string dummyPassword, IList<string> roles)
		{
			if (emailTemplate.Contains(EmailTemplateConstants.DummyPassword))
			{
				emailTemplate = emailTemplate.Replace(EmailTemplateConstants.DummyPassword, dummyPassword);
			}

			if (emailTemplate.Contains(EmailTemplateConstants.EmailConfirmationTemplateConstant))
			{
				emailTemplate =
					emailTemplate.Replace(EmailTemplateConstants.EmailConfirmationTemplateConstant, callbackUrl);
			}

			if (emailTemplate.Contains(EmailTemplateConstants.PasswordResetTemplateConstant))
			{
				emailTemplate =
					emailTemplate.Replace(EmailTemplateConstants.PasswordResetTemplateConstant, callbackUrl);
			}

			if (emailTemplate.Contains(EmailTemplateConstants.UserNameTemplateConstant))
			{
				var userName = user.AppUser.Email.Split('@')[0];
				if (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.LastName))
				{
					userName = !string.IsNullOrEmpty(user.FirstName) ? user.FirstName : user.LastName;
				}

				emailTemplate =
					emailTemplate.Replace(EmailTemplateConstants.UserNameTemplateConstant, userName);
			}

			if (emailTemplate.Contains(EmailTemplateConstants.UserEmailTemplateConstant))
			{
				emailTemplate =
					emailTemplate.Replace(EmailTemplateConstants.UserEmailTemplateConstant, user.AppUser.Email);
			}

			if (emailTemplate.Contains(EmailTemplateConstants.UserRoleTemplateConstant))
			{
				string rolesAsString = "_NONE_";
				if (roles != null && roles.Count > 0)
				{
					rolesAsString = "";
					string lastElement = roles[roles.Count - 1];
					foreach (var role in roles)
					{
						if (role != lastElement)
							rolesAsString += role + ", ";
						else
							rolesAsString += role;
					}
				}

				emailTemplate =
					emailTemplate.Replace(EmailTemplateConstants.UserRoleTemplateConstant, rolesAsString);
			}
			if (emailTemplate.Contains(EmailTemplateConstants.SetupPasswordCodeTemplateConstant))
			{
				emailTemplate =
					emailTemplate.Replace(EmailTemplateConstants.SetupPasswordCodeTemplateConstant, callbackUrl);
			}

			return emailTemplate;
		}
	}
}