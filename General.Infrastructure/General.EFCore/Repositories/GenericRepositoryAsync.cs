using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Core.Repositories;
using General.Infrastructure.EFCore.EntityContext;
using Microsoft.EntityFrameworkCore;

namespace General.Infrastructure.EFCore.Repositories
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T>
        where T : class
    {
        private readonly RowEntityContext dbContext;

        public GenericRepositoryAsync(RowEntityContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            var entity = await dbContext.Set<T>().FindAsync(id);
            dbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            var entity = await dbContext.Set<T>().FindAsync(id);
            return entity;
        }

        public async Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
        {
            return await dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            return entity;
        }

        public async Task<int> AddRangeAsync(List<T> entity)
        {
            var result = 0;
            await dbContext.Set<T>().AddRangeAsync(entity);
            try
            {
                result = await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            return result;
        }

        public async Task UpdateAsync(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateBulkAsync(List<T> entities)
        {
            foreach (var entity in entities)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await dbContext
                 .Set<T>()
                 .ToListAsync();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return dbContext
                 .Set<T>();
        }

        public async Task ExexuteScriptAsync(string script)
        {
            await dbContext.Database.ExecuteSqlRawAsync(script);
        }
    }
}
