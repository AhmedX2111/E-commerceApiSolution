using Ecommerce.Domainn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Interfaces.Repositories
{
	public interface IBaseRepository<T> where T : BaseEntity
	{
		Task<T?> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task<bool> ExistsAsync(int id);
	}
}
