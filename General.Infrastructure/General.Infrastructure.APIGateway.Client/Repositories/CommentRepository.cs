using Microsoft.EntityFrameworkCore;
using General.Core.Entities;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;
using General.Infrastructure.EFCore.Repositories;

namespace General.Infrastructure.APIGateway.Client.Repositories.LeaseServiceRepos
{
	public class CommentRepository : GenericRepositoryAsync<Comment>, ICommentRepository
	{
		private readonly DbSet<Comment> _context;
		public CommentRepository(RowEntityContext dbContext) : base(dbContext)
		{
			_context = dbContext.Set<Comment>();
		}
	}
}
