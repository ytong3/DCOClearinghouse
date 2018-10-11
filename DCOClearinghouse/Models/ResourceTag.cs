namespace DCOClearinghouse.Models
{
    public class ResourceTag
    {
        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
        public int TagID { get; set; }
        public Tag Tag { get; set; }
    }
}