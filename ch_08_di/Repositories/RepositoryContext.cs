using Microsoft.EntityFrameworkCore;

namespace ch_08_di.Repositories
{
    public class RepositoryContext:DbContext
    {
        public DbSet<Book> Books { get; set; }
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().HasData
                (
                new Book { Id = 1, Title = "İnce Memed", Price = 20 },
                new Book { Id = 2, Title = "Kuyucaklı Yusuf", Price = 15.5M },
                new Book { Id = 3, Title = "Çalıkuşu", Price = 18.75M }
                );
        }
    }
}
