using Ecommerce.Domainn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Interfaces.Repositories
{
	public interface IProductRepository : IBaseRepository<Product>
	{
		Task<Product?> GetByProductCodeAsync(string productCode);
		Task<IEnumerable<Product>> GetByCategoryAsync(string category);
		Task<bool> ProductCodeExistsAsync(string productCode);
	}
}
