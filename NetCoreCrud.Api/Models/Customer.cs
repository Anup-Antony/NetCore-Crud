using System.ComponentModel.DataAnnotations;

namespace NetCoreCrud.Api.Models
{
    public class Customer
    {
        [Key]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
    }
}
