using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Dto.Token
{
    public class JWTDto
    {
        public string Token{ get; set; }
        public DateTime Expiration{ get; set; }
    }
}
