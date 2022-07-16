using System;
using System.Collections.Generic;

namespace General.Core.Application.Feature.DTO
{
	public class CommentDto
	{
		public long Id { get; set; }
		public long EntityTypeId { get; set; }
		public string UserId { get; set; }
		public string Description { get; set; }
		public DateTime CreatedOn { get; set; }
		public string UserName { get; set; }
	}

	public class CommentListDto
	{
		public long TotalCount { get; set; }
		public List<CommentDto> Items { get; set; }
	}
}
