using InterviewMauiBlazor.Database.Entities;
using InterviewMauiBlazor.Database.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewMauiBlazor.Database.Repositories
{
    public interface IOrderRepositories : IRepository<Order>
    {

    }
    public class OrderRepository : RepositoryBase<Order>, IOrderRepositories
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
