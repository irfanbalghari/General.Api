using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using General.Core.Application.Dto.AppUser;
using Newtonsoft.Json;

namespace General.Core.Application.Feature.UserManagement.DTO
{
    public class UserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> RoleList { get; set; }
        [JsonIgnore]
        internal RolesDto Roles { get; set; }
        public string  OfficeAddress { get; set; }
    }
}
