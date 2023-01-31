using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PracticeSession3.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "enter name with maximum 50 character")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "enter valid email address")]
        [Required]
        public string Email { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();


    }
}
