using System;

namespace General.Core.Application.Feature.Model
{
	public class CommentModel
	{
		public long Id { get; set; }
		public long EntityTypeId { get; set; }
		public string UserId { get; set; }
		public string Description { get; set; }
		public DateTime CreatedOn { get; set; }
	}
}
