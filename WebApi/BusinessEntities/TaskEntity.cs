using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEntities
{
    public class TaskEntity
    {
        public int id { get; set; }
        public int? projectId { get; set; }
        public string taskName { get; set; }
        public string projectName { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public bool isParent { get; set; }
        public int? priority { get; set; }
        public int? parentTaskId { get; set; }   
        public int? status { get; set; }

        public int? contactId { get; set; }
        public string contactName { get; set; }
    }
}

 
 
