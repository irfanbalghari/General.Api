using General.Cars.Core.Entities;

namespace General.Core.Entities
{
	public class Attachment : BaseEntity
	{
		public long Id { get; set; }
		public string AttachementID { get; set; }
		public string Name { get; set; }
		public string Location { get; set; }
		public string URL { get; set; }
		public long FileSize { get; set; }
		public string FileExtension { get; set; }
		public string PreviewName { get; set; }
	}
}
