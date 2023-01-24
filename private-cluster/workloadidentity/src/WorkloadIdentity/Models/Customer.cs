using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkloadIdentity.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        public string Id { get; set; }
        public int SourceId { get; set; }
        public string Company { get; set; }
        public string Level { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
    }
}
