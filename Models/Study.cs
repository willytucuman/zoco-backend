using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendZocoUsers.Models
{
    public class Study
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Institution { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
