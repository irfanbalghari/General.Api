using General.Core.Entities;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;
using General.Infrastructure.EFCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace General.Cars.Infrastructure.EFCore.Repositories
{
	public class AttachmentRepository : GenericRepositoryAsync<Attachment>, IAttachmentRepository
	{
		private readonly DbSet<Attachment> _context;
		public AttachmentRepository(GeneralEntityContext context) : base(context)
		{
			_context = context.Set<Attachment>();
		}
	}
}
