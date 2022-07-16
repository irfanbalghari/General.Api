using General.Common.Logging;
using General.Core.Application.Dto.Token;
using General.Core.Application.Interfaces.Auth;
using General.Core.Application.Wrappers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Services.Auth
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration configuration;
      
        private readonly ILogger logger;
        public TokenService(IConfiguration configuration, ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public TokenService()
        {
         
        }


        public Response<JWTDto> GenerateToken(List<Claim> authClaims)
        {
            Response<JWTDto> response = new Response<JWTDto>() { Success = true };
            try
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
                var token = new JwtSecurityToken(
                          issuer: configuration["JWT:ValidIssuer"],
                          audience: configuration["JWT:ValidAudience"],
                          expires: DateTime.Now.AddHours(6),
                          claims: authClaims,
                          signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                JWTDto jWTDto = new JWTDto
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };
                response.Data = jWTDto;
            }
            catch (Exception ex)
            {
                logger.Client.TrackException(ex);
                response.Errors.Add(ex.Message);
                response.Success = false;
            }
            return response;
        }


    }
}
