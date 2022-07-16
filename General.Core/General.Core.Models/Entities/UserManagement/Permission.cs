using General.Cars.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace General.Core.Entities.UserManagement
{
	public class Permission 
	{
		[Key]
		public long Id { get; set; }
		public string PermissionName { get; set; }
		public string PermissionCode { get; set; }
		public string Category { get; set; }
	}
}
