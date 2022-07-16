using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core
{
    public struct AppConstants
    {
        public static string Success = "Success";
        public static string Failed = "Failed";
        public struct UserManagement
        {
            public static string UserExists = "User already exists";
            public static string UserNotExists = "User does not exists";
            public static string InvalidUserName = "InvalidUserName";
            public static string IncorrectPassword = "IncorrectPassword";
        }
        public struct MicroServiceConfig
        {
            public const string Cars = "Cars";
            public static string Leases = "Leases";
        }

        public struct UserSorting
		{
            public const string Email = "Eamil";
            public const string CreatedOn = "CreatedOn";
        }
        public struct Sorting
        {
            public const string ASC = "asc";
            public const string DESC = "desc";
        }
    }
}
