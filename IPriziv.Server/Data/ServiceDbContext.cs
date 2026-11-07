using IPriziv.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IPriziv.Server.Data
{
    public class ServiceDbContext : DbContext
    {
        public DbSet<ApiKey> ApiKeys { get; set; }
        public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
