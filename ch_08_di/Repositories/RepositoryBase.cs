namespace ch_08_di.Repositories
{
    public abstract class RepositoryBase<T> where T : class
    {
        protected readonly RepositoryContext _context;

        protected RepositoryBase(RepositoryContext context)
        {
            _context = context;
        }

        public virtual T? Get(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public virtual ICollection<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

    }
}
