using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace BackendZocoUsers.Models
{
    public enum Role
    {
        Admin,
        User
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; } = Role.User;

        public ICollection<Study>? Studies { get; set; }
        public ICollection<Address>? Addresses { get; set; }
        public ICollection<SessionLog>? SessionLogs { get; set; }
    }
}
