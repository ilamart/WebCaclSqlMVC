using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class CalculationDbContext : DbContext
    {
        public DbSet<History> Histories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(@"Data Source=BAS\LATIN;Initial Catalog=calculate_loc;User ID=sa1;Password=111;ApplicationIntent=ReadWrite;");
        }
    }
}