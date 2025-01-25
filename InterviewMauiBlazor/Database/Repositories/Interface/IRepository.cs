using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InterviewMauiBlazor.Database.Repositories.Interface
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        void ExecuteDelete(Expression<Func<T, bool>> where);
        int Count(Expression<Func<T, bool>> where);

        IEnumerable<T> GetList(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int skip = 0,
            int take = 0);

        T GetById(object Id);

        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);

        bool Any(Expression<Func<T, bool>> where);
    }
    public abstract class RepositoryBase<T> where T : class
    {
        protected ApplicationDbContext _dbContext;
        protected readonly DbSet<T> dbset;

        protected RepositoryBase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            dbset = _dbContext.Set<T>();
        }

        public virtual void Add(T entity)
        {            
            dbset.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            dbset.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            dbset.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            dbset.Remove(entity);
        }

        public virtual void ExecuteDelete(Expression<Func<T,bool>> where)
        {
            dbset.Where(where).ExecuteDelete();
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            dbset.RemoveRange(entities);
        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbset.Remove(obj);
        }

        public virtual int Count(Expression<Func<T, bool>> where)
        {
            return dbset.Count<T>(where);
        }

        public virtual IEnumerable<T> GetList(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int skip = 0,
            int take = 0)
        {
            IQueryable<T> query = dbset;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip > 0)
            {
                query = query.Skip(skip);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            return query.ToList();
        }

        public virtual T GetById(object id)
        {
            return dbset.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbset.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).ToList();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).FirstOrDefault<T>();
        }

        public virtual bool Any(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).Any<T>();
        }
    }
}
