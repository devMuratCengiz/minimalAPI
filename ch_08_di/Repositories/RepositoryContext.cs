using Microsoft.EntityFrameworkCore;

namespace ch_08_di.Repositories
{
    public class RepositoryContext:DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Roman" },
                new Category { Id = 2, Name = "Hikaye" },
                new Category { Id = 3, Name = "Deneme" }
                );

            modelBuilder.Entity<Book>().HasData
                (
                new Book { Id = 1, CategoryId=1,Url = "/images/default1.jpg" , Title = "İnce Memed", Price = 20 },
                new Book { Id = 2, CategoryId = 2 , Url = "/images/default2.jpg" , Title = "Kuyucaklı Yusuf", Price = 15.5M },
                new Book { Id = 3,CategoryId = 3, Url = "/images/default3.jpg" , Title = "Çalıkuşu", Price = 18.75M }
                );
        }
    }
}
