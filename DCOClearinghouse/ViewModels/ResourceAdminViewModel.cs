using DCOClearinghouse.Models;

namespace DCOClearinghouse.ViewModels
{
    public class ResourceAdminViewModel
    {
        public Resource Resource { get; set; }
        public string NewCategoryName { get; set; }
        public string NewTypeName { get; set; }
    }
}