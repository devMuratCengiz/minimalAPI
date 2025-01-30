namespace ch_08_di.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>
    {
        public CategoryRepository(RepositoryContext context) : base(context)
        {
        }
    }
}
