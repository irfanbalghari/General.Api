using General.Cars.Core.Entities;
using System;

namespace General.Core.Entities
{
	public class Comment : BaseEntity
	{
		public long Id { get; set; }
		public long EntityTypeId { get; set; }
		public string UserId { get; set; }
		public string Description { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
