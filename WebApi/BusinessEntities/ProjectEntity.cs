using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEntities
{
   public class ProjectEntity
    {
        public int id { get; set; }
        public string projectName { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public bool isSetDate { get; set; }
        public int? priority { get; set; }
        public int? contactId { get; set; }
        public string projectManager { get; set; }
    }
}
 