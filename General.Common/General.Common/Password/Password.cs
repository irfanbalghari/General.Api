using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Common.Password
{
    public class Password
    {
        public bool RequireDigit { get; set; }
        public int Length { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowerCase { get; set; }
        public int RequireUniqueChars { get; set; }
        public int LockoutTimeInMin { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
        public bool LockoutAllowForNewUsers { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
}
