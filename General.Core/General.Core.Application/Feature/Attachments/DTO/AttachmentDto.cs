using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Application.Feature.Attachments.DTO
{
    public class AttachmentDto
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
