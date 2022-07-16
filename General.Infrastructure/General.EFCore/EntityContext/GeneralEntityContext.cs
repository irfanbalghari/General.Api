
using General.Cars.Core.Entities;
using General.Core.Entities;
using General.Core.Entities.UserManagement;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace General.Infrastructure.EFCore.EntityContext
{
	public class GeneralEntityContext : IdentityDbContext<ApplicationUser>
	{
		public DbSet<UserProfile> UserProfile { get; set; }
		public GeneralEntityContext(DbContextOptions<GeneralEntityContext> options)
			  : base(options)
		{

		}
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Comment> Comments { get; set; }
		//public DbSet<Attachment> Attachment { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<RolePermission>().HasKey(rp => new { rp.PermissionId, rp.RoleId });
		}
		[NotMapped]
		public DbSet<SPUserList> SPUserList { get; set; }
	}

}
