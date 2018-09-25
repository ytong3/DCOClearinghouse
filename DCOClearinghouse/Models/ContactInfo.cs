using System.ComponentModel.DataAnnotations;

namespace DCOClearinghouse.Models
{
    public class ContactInfo
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Tel")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }

        [Display(Name = "State")]
        [RegularExpression("[A-Z]{2}")]
        public string State { get; set; }
        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }
    }
}