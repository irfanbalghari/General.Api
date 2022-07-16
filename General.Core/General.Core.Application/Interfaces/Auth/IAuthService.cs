using General.Core.Application.Dto.AppUser;
using General.Core.Application.Dto.Token;
using General.Core.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Response<JWTDto>> LoginAsync(LoginDto loginDto);
    }
}
