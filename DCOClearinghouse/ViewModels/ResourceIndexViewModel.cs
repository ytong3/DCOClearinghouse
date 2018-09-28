using System.Collections.Generic;
using DCOClearinghouse.Models;

namespace DCOClearinghouse.ViewModels
{
    public class ResourceIndexViewModel
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<ResourceCategory> Children { get; set; }
    }
}