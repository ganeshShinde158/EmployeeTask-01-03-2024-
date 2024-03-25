using Microsoft.EntityFrameworkCore;

namespace EmployeeTask_01_03_2024_.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeModel> employees { get; set; }
    }
}
