using System;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(60, MinimumLength = 2)]
        [Required]
        [Display(Name = "Organization")]
        public string Subject { get; set; }

        [DataType(DataType.Url)]
        public string Link { get; set; }

        [StringLength(300, MinimumLength = 3)]
        [Required]
        public string Content { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "{0} is required.")]
        public int? CategoryID { get; set; }
        [Display(Name = "Type")]
        [Required(ErrorMessage = "{0} is required.")]
        public int? TypeID { get; set; }

        [Display(Name = "Marked as bad link")]
        public int BadlinkVotes { get; set; }

        [Display(Name = "Date")]
        [DataType((DataType.Date))]
        public DateTime CreateDate { get; set; }

        [EnumDataType(typeof(ResourceStatus))]
        public ResourceStatus Status { get; set; }

        public ContactInfo Contact { get; set; }
        public Boolean IsContactInfoPublic { get; set; }

        #region Navigation properties

        public ResourceCategory Category { get; set; }

        public ResourceType Type { get; set; }

        #endregion

    }
}