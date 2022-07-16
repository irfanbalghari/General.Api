using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Repositories
{
    public interface IGenericRepositoryAsync<T>
        where T : class
    {
        Task<T> GetByIdAsync(long id);

        Task<T> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllAsync();

        IQueryable<T> GetAllQueryable();

        Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

        Task<T> AddAsync(T entity);

        Task<int> AddRangeAsync(List<T> entity);

        Task UpdateAsync(T entity);

        Task UpdateBulkAsync(List<T> entities);

        Task DeleteAsync(T entity);

        Task ExexuteScriptAsync(string script);
    }
}
