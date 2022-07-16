using System.Collections.Generic;

namespace General.Core.Application.Feature.UserManagement.Model
{
	public class UserSearchModel
	{
        public int StartRowIndex { get; set; }
        public int MaxRows { get; set; }
        public string SortDirection { get; set; }
        public string SortBy { get; set; }
        public string SearchKeyword { get; set; }
        public List<string> RoleIds { get; set; }
    }
}
