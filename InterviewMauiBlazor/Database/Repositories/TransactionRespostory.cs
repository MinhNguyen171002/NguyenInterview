using InterviewMauiBlazor.Database.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterviewMauiBlazor.Database.Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace InterviewMauiBlazor.Database.Repositories
{
    public interface ITransactionRepositories : IRepository<Transaction>
    {
        Transaction getByKeys(object proid,object orderid);
    }
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepositories
    {
        public TransactionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
        public Transaction getByKeys(object proid, object orderid)
        {
            return dbset.Find(proid, orderid);
        }        
    }
    
}
