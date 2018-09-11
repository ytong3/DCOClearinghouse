using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCOClearinghouse.Models
{
    public enum ResourceStatus
    {
        New = 0,
        Approved,
        Removed
    }
    public class Resource
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int CategoryID { get; set; }
        public int BadlinkVotes { get; set; }
        public DateTime CreateDate { get; set; }
        public ResourceStatus Status { get; set; }

        public ResourceCategory Category { get; set; }
    }
}