using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeekCoding.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected EvaluatorContext RepositoryContext { get; set; }
        public RepositoryBase(EvaluatorContext repoContext)
        {
            RepositoryContext = repoContext;
        }

        #region add
        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }

        public virtual T Add(T t)
        {

            RepositoryContext.Set<T>().Add(t);
            RepositoryContext.SaveChanges();
            return t;
        }

        public virtual async Task<T> AddAsync(T t)
        {
            RepositoryContext.Set<T>().Add(t);
            await RepositoryContext.SaveChangesAsync();
            return t;

        }
        #endregion

        #region find
        public virtual T Find(Expression<Func<T, bool>> match)
        {
            return RepositoryContext.Set<T>().SingleOrDefault(match);
        }
        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return RepositoryContext.Set<T>().Where(expression);
        }

        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await RepositoryContext.Set<T>().SingleOrDefaultAsync(match);
        }
        #endregion

        #region get
        public T GetItem(Guid id)
        {
            return RepositoryContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetAsync(Guid id)
        {
            return await RepositoryContext.Set<T>().FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return RepositoryContext.Set<T>();
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            return await RepositoryContext.Set<T>().ToListAsync();
        }

        #endregion

        #region update
        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }

        public virtual async Task<T> UpdateAsync(T t, object key)
        {
            if (t == null)
                return null;
            T exist = await RepositoryContext.Set<T>().FindAsync(key);
            if (exist != null)
            {
                RepositoryContext.Entry(exist).CurrentValues.SetValues(t);
                await RepositoryContext.SaveChangesAsync();
            }
            return exist;
        }
        #endregion

        #region delete
        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public void Delete(Guid entityId)
        {
            RepositoryContext.Set<T>().Remove(RepositoryContext.Set<T>()?.Find(entityId));
        }

        public virtual async Task<int> DeleteAsync(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
            return await RepositoryContext.SaveChangesAsync();
        }
        #endregion

        public void Save()
        {
            RepositoryContext.SaveChanges();
        }

        public virtual async Task SaveAsync()
        {
           await RepositoryContext.SaveChangesAsync();
        }
    }
}
