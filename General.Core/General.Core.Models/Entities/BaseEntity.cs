using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Cars.Core.Entities
{
    public class BaseEntity
    {
        [MaxLength(250)]
        public string CreatedBy { get; set; }
        [MaxLength(250)]
        public string ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool Deleted { get; set; }
    }
}
