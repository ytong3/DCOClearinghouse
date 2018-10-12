using System;
using System.Collections.Generic;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [StringLength(200, MinimumLength = 1)]
        [Required]
        [Display(Name = "Title")]
        public string Subject { get; set; }

        [UIHint("NewWindowUrl"), DataType(DataType.Url)]
        public string Link { get; set; }

        [StringLength(300, MinimumLength = 3)]
        public string Description { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        [Display(Name = "Type")]
        public int? TypeID { get; set; }

        [Display(Name = "Marked as bad link")]
        public int BadlinkVotes { get; set; }

        [Display(Name = "Date")]
        [DataType((DataType.Date))]
        public DateTime? CreateDate { get; set; }

        [EnumDataType(typeof(ResourceStatus))]
        public ResourceStatus Status { get; set; }

        public ContactInfo Contact { get; set; }
        

        #region Navigation properties

        public ResourceCategory Category { get; set; }

        public ResourceType Type { get; set; }

        [Display(Name="Tags")]
        public ICollection<ResourceTag> ResourceTags { get; set; }
        #endregion

    }
}