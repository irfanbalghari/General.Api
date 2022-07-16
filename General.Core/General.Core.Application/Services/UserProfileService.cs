using General.Core.Application.Dto;
using General.Core.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Services
{
    public class UserProfileService : IUserProfile
    {
        IHttpContextAccessor httpContextAccessor;
        public UserProfileService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public UserProfile GetUserProfile()
        {

            UserProfile profile = new UserProfile();
            try
            {
                if (httpContextAccessor != null
                    && httpContextAccessor.HttpContext != null
                    && httpContextAccessor.HttpContext.User != null
                    && httpContextAccessor.HttpContext.User.Claims != null
                    )
                {
                    var claims = httpContextAccessor.HttpContext.User.Claims.ToList();
                    profile.Id = claims?.FirstOrDefault(x => x.Type.Equals("Id", StringComparison.OrdinalIgnoreCase))?.Value;
                    profile.Email = claims?.FirstOrDefault(x => x.Type.Equals("Email", StringComparison.OrdinalIgnoreCase))?.Value;
                    profile.Name = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))?.Value;
                }
            }
            catch
            {

            }
            return profile;
        }
    }
}
