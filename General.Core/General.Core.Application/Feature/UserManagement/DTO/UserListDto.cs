using System;
using System.Collections.Generic;

namespace General.Core.Application.Feature.UserManagement.DTO
{
	public class UserListDto
	{
		public long TotalCount { get; set; }
		public List<ListUserDto> Items { get; set; }
	}
    public class ListUserDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }
        public string OfficeAddress { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
