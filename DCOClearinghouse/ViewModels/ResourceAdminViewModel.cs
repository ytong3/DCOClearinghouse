using DCOClearinghouse.Models;

namespace DCOClearinghouse.ViewModels
{
    public class ResourceAdminViewModel
    {
        public Resource Resource { get; set; }
        public ResourceCategory NewCategory { get; set; }
        public string NewTypeName { get; set; }
    }
}