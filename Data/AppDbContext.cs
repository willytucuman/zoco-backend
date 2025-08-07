using BackendZocoUsers.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BackendZocoUsers.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Study> Studies => Set<Study>();
        public DbSet<SessionLog> SessionLogs => Set<SessionLog>();
    }
}
