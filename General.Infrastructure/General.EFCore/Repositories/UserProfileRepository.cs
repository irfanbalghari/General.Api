using Microsoft.EntityFrameworkCore;
using General.Core.Entities;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;

namespace General.Infrastructure.EFCore.Repositories
{
	public class UserProfileRepository : GenericRepositoryAsync<UserProfile>, IUserRepository
	{
		private readonly DbSet<UserProfile> _context;

		public UserProfileRepository(RowEntityContext dbContext) : base(dbContext)
		{
			_context = dbContext.Set<UserProfile>();
		}
	}
}
