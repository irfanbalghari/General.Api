using Microsoft.EntityFrameworkCore;
using General.Core.Entities;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;
using General.Infrastructure.EFCore.Repositories;

namespace General.Cars.Infrastructure.EFCore.Repositories
{
	public class AttachmentRepository : GenericRepositoryAsync<Attachment>, IAttachmentRepository
	{
		private readonly DbSet<Attachment> _context;
		public AttachmentRepository(RowEntityContext context) : base(context)
		{
			_context = context.Set<Attachment>();
		}
	}
}
