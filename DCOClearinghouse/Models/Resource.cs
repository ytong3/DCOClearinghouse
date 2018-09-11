using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCOClearinghouse.Models
{
    public enum ResourceStatus
    {
        New = 0,
        Approved,
        Removed
    }
    public class Resource
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }

        [Display(Name = "Marked as bad link")]
        public int BadlinkVotes { get; set; }
        public DateTime CreateDate { get; set; }

        [EnumDataType(typeof(ResourceStatus))]
        public ResourceStatus Status { get; set; }

        public ResourceCategory Category { get; set; }
    }
}