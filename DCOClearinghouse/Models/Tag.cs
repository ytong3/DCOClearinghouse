using System;
using System.Collections.Generic;

namespace DCOClearinghouse.Models
{
    public class Tag
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public ICollection<ResourceTag> ResourceTags { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}