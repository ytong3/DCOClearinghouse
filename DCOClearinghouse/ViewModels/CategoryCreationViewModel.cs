using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DCOClearinghouse.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCOClearinghouse.ViewModels
{
    public class CategoryCreationViewModel
    {
        public List<ResourceCategory> ParentCategories { get; set; }
        public int ParentCategoryID { get; set; }
        public bool IsNewParentCategory { get; set; }
        public string NewParentCategoryName { get; set; }
        [Required]
        public string  NewSubcategoryName { get; set; }

    }
}