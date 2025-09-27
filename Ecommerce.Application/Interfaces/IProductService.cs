using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs.ProductDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Interfaces
{
	public interface IProductService
	{
		Task<Result<IEnumerable<ProductDto>>> GetAllProductsAsync();
		Task<Result<ProductDto>> GetProductByIdAsync(int id);
		Task<Result<ProductDto>> CreateProductAsync(CreateProductRequest request, string imagePath);
		Task<Result<ProductDto>> UpdateProductAsync(int id, UpdateProductRequest request, string? imagePath);
		Task<Result<bool>> DeleteProductAsync(int id);
	}
}
