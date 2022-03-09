using Microsoft.EntityFrameworkCore;
using OngProject.DataAccess;
using OngProject.Entities;
using OngProject.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace OngProject.Repositories
{
    public class Repository<T> : IRepository<T> where T : EntityBase
    {
        protected AppDbContext RepositoryContext { get; set; }
        protected DbSet<T> dbSet;
        public Repository(AppDbContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
            dbSet = repositoryContext.Set<T>();
        }
        public async Task<ICollection<T>> FindAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<ICollection<T>> FindAllAsync(
             Expression<Func<T, bool>> filter = null,
             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
             IList<Expression<Func<T, object>>> includes = null,
             int? page = null,
             int? pageSize = null)
        {
            var query = this.dbSet.AsQueryable();

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<int> Count()
        {
            return await dbSet.CountAsync();
        }

        public async Task<ICollection<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await dbSet.Where(expression).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await this.dbSet.FindAsync(id);
        }

        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
