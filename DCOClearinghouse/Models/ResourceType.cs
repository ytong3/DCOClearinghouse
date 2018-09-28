using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCOClearinghouse.Models
{
    public class ResourceType
    {
        public int? ID { get; set; }
        public string TypeName { get; set; }

        #region Navigation property
        public ICollection<Resource> Resources { get; set; }
        #endregion
    }
}