using InterviewMauiBlazor.Database.Entities;
using InterviewMauiBlazor.Database.Repositories.Interface;


namespace InterviewMauiBlazor.Database.Repositories
{
    public interface IProductRepositories : IRepository<Product>
    {
    }
    public class ProductRepository : RepositoryBase<Product>, IProductRepositories
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
