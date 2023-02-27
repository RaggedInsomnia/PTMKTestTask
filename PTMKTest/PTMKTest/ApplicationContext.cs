using Microsoft.EntityFrameworkCore;

namespace PTMKTest
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons => Set<Person>();

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=persondb;Trusted_Connection=True;");
        }
    }
}
