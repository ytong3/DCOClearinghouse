using System.ComponentModel.DataAnnotations.Schema;

namespace DCOClearinghouse.Models
{
    public class ResourceCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string CategoryName { get; set; }
    }
}