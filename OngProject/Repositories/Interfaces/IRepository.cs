using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OngProject.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        Task<ICollection<T>> FindAllAsync();
        Task<ICollection<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<ICollection<T>> FindAllAsync(
             Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             IList<Expression<Func<T, object>>> includes = null,
             int? page = null,
             int? pageSize = null);
        Task<int> Count();
        Task<T> GetByIdAsync(int id);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
