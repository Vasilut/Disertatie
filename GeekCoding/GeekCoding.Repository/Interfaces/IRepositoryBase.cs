using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeekCoding.Repository.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        #region get
        IQueryable<T> GetAll();
        Task<ICollection<T>> GetAllAsync();
        T GetItem(Guid id);
        Task<T> GetAsync(Guid id);
        #endregion

        #region find
        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        T Find(Expression<Func<T, bool>> match);
        #endregion

        #region add
        void Create(T entity);
        Task<T> AddAsync(T t);
        #endregion

        #region update
        void Update(T entity);
        Task<T> UpdateAsync(T t, object key);
        #endregion

        #region delete
        void Delete(T entity);
        void Delete(Guid entityId);
        Task<int> DeleteAsync(T entity);
        #endregion

        #region save
        void Save();
        Task SaveAsync();
        #endregion
    }
}
