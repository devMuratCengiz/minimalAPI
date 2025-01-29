namespace ch_08_di.Repositories
{
    public class BookRepository
    {
        private readonly RepositoryContext _context;

        public BookRepository(RepositoryContext context)
        {
            _context = context;
        }

        public Book? Get(int id)
        {
            return _context.Books.FirstOrDefault(x => x.Id == id);
        }

        public List<Book> GetAll()
        {
            return _context.Books.ToList();
        }
        
        public void Add(Book item)
        {
            _context.Books.Add(item);
            _context.SaveChanges();
        }
        public void Remove(Book item)
        {
            _context.Books.Remove(item);
            _context.SaveChanges();
        }

        public void Update(Book item)
        {
            _context.Books.Update(item);
            _context.SaveChanges();
        }
    }
}
