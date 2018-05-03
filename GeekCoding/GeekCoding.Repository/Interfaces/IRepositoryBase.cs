using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GeekCoding.Repository.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        IQueryable<T> GetAll();
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        T GetItem(Guid id);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid entityId);
        void Save();
    }
}
