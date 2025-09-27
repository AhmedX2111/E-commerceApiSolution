using Ecommerce.Domainn.Entities;
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
	public class ProductRepository : BaseRepository<Product>, IProductRepository
	{
		public ProductRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<Product?> GetByProductCodeAsync(string productCode)
		{
			return await _context.Products
				.FirstOrDefaultAsync(p => p.ProductCode == productCode);
		}

		public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
		{
			return await _context.Products
				.Where(p => p.Category == category)
				.ToListAsync();
		}

		public async Task<bool> ProductCodeExistsAsync(string productCode)
		{
			return await _context.Products
				.AnyAsync(p => p.ProductCode == productCode);
		}
	}
}
