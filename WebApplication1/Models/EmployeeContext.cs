using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class EmployeeContext:DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options):base(options)
        {

        }

        public DbSet<Employee> Employees {get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }

    }
}
