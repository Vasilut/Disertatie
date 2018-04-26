using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GeekCoding.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected EvaluatorContext RepositoryContext { get; set; }
        public RepositoryBase(EvaluatorContext repoContext)
        {
            RepositoryContext = repoContext;
        }

        public IEnumerable<T> FindAll()
        {
            return RepositoryContext.Set<T>();
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return RepositoryContext.Set<T>().Where(expression);
        }

        public void Create(T entity)
        {
            RepositoryContext.Set<T>().Add(entity);
        }

        public T GetItem(Guid id)
        {
            return RepositoryContext.Set<T>().Find(id);
        }

        public void Update(T entity)
        {
            RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Set<T>().Remove(entity);
        }

        public void Delete(Guid entityId)
        {
            RepositoryContext.Set<T>().Remove(RepositoryContext.Set<T>()?.Find(entityId));
        }

        public void Save()
        {
            RepositoryContext.SaveChanges();
        }
    }
}
