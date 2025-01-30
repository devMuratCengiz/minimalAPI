using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ch_08_di.Repositories
{
    public class BookRepository : RepositoryBase<Book>
    {
        public BookRepository(RepositoryContext context) : base(context)
        {
        }

        public override Book? Get(int id)
        {
            return _context.Books.Include(b=>b.Category).FirstOrDefault(b=>b.Id == id);
        }

        public override ICollection<Book> GetAll()
        {
            return _context.Books.Include(_b => _b.Category).ToList();
        }
    }
}
