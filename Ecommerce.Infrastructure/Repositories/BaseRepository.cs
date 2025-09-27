using Ecommerce.Domainn.Common;
using Ecommerce.Domainn.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
	{
		protected readonly ApplicationDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public BaseRepository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public virtual async Task<T?> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public virtual async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public virtual async Task<T> AddAsync(T entity)
		{
			_dbSet.Add(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public virtual async Task UpdateAsync(T entity)
		{
			entity.UpdatedAt = DateTime.UtcNow;
			_dbSet.Update(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task<bool> ExistsAsync(int id)
		{
			return await _dbSet.AnyAsync(e => e.Id == id);
		}
	}
}
