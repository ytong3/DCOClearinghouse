using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCOClearinghouse.Models
{
    public class ResourceCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CategoryName { get; set; }
        [Display(Name = "Parent Category")]
        public int? ParentCategoryID { get; set; }
        public int Depth { get; set; }

        #region Navigation property
        public virtual ResourceCategory ParentCategory { get; set; }
        public virtual ICollection<ResourceCategory> ChildrenCategories { get; set; }
        public ICollection<Resource> Resources { get; set; }
        #endregion
    }
}