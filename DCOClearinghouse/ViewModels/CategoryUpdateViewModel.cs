using System.Collections.Generic;
using DCOClearinghouse.Models;

namespace DCOClearinghouse.ViewModels
{
    public class CategoryUpdateViewModel
    {
        public List<ResourceCategory> ParentCategories { get; set; }
        public ResourceCategory CategoryToUpdate {get; set; }
    }
}