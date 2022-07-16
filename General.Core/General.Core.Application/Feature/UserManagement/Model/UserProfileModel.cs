

using General.Core.Application.Dto.AppUser;

namespace General.Core.Application.Feature.UserManagement.Model
{
    public class UserProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public RolesDto Roles { get; set; }
        public string OfficeAddress { get; set; }
    }
}
