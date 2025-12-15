using System.ComponentModel.DataAnnotations;

namespace LegalEagles.Web.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = "";

        [Required, StringLength(50)]
        public string LastName { get; set; } = "";

        [Required, StringLength(200)]
        public string Address { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, Phone]
        public string MobileNumber { get; set; } = "";

        [Required, DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
