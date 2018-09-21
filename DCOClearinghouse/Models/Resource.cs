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
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Subject { get; set; }

        [DataType(DataType.Url)]
        public string Link { get; set; }

        [StringLength(300, MinimumLength = 3)]
        [Required]
        public string Content { get; set; }
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        [Display(Name = "Type")]
        public int TypeID { get; set; }

        [Display(Name = "Marked as bad link")]
        public int BadlinkVotes { get; set; }

        [Display(Name = "Date")]
        [DataType((DataType.Date))]
        public DateTime CreateDate { get; set; }

        [EnumDataType(typeof(ResourceStatus))]
        public ResourceStatus Status { get; set; }

        public ResourceCategory Category { get; set; }

        public ResourceType Type { get; set; }
    }
}