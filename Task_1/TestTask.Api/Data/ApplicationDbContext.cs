using Microsoft.EntityFrameworkCore;

namespace TestTask.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CodeValue> CodeValues { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    }
}
