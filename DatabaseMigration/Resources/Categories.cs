namespace DatabaseMigration.Resources
{
    public partial class Categories
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int? Lft { get; set; }
        public int? Rgt { get; set; }
        public string Name { get; set; }
    }
}
