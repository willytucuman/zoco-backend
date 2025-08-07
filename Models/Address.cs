using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendZocoUsers.Models
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
