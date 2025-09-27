using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domainn.Entities;
using Ecommerce.Domainn.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services
{
		public class ProductService : IProductService
		{
			private readonly IProductRepository _productRepository;

			public ProductService(IProductRepository productRepository)
			{
				_productRepository = productRepository;
			}

			public async Task<Result<IEnumerable<ProductDto>>> GetAllProductsAsync()
			{
				try
				{
					var products = await _productRepository.GetAllAsync();
					var productDtos = products.Select(p => MapToDto(p));
					return Result<IEnumerable<ProductDto>>.Success(productDtos);
				}
				catch (Exception ex)
				{
					return Result<IEnumerable<ProductDto>>.Failure($"Error retrieving products: {ex.Message}");
				}
			}

			public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
			{
				try
				{
					var product = await _productRepository.GetByIdAsync(id);
					if (product == null)
						return Result<ProductDto>.Failure("Product not found");

					return Result<ProductDto>.Success(MapToDto(product));
				}
				catch (Exception ex)
				{
					return Result<ProductDto>.Failure($"Error retrieving product: {ex.Message}");
				}
			}

			public async Task<Result<ProductDto>> CreateProductAsync(CreateProductRequest request, string imagePath)
			{
				try
				{
					// Check if product code would be unique
					var productCode = await GenerateUniqueProductCodeAsync();

					var existingProduct = await _productRepository.GetByProductCodeAsync(productCode);
					if (existingProduct != null)
						return Result<ProductDto>.Failure("Product code already exists");

					var product = new Product
					{
						Category = request.Category,
						ProductCode = productCode,
						Name = request.Name,
						ImagePath = imagePath,
						Price = request.Price,
						MinimumQuantity = request.MinimumQuantity,
						DiscountRate = request.DiscountRate,
						CreatedAt = DateTime.UtcNow
					};

					var createdProduct = await _productRepository.AddAsync(product);
					return Result<ProductDto>.Success(MapToDto(createdProduct));
				}
				catch (Exception ex)
				{
					return Result<ProductDto>.Failure($"Error creating product: {ex.Message}");
				}
			}

			public async Task<Result<ProductDto>> UpdateProductAsync(int id, UpdateProductRequest request, string? imagePath)
			{
				try
				{
					var product = await _productRepository.GetByIdAsync(id);
					if (product == null)
						return Result<ProductDto>.Failure("Product not found");

					product.Category = request.Category;
					product.Name = request.Name;
					product.Price = request.Price;
					product.MinimumQuantity = request.MinimumQuantity;
					product.DiscountRate = request.DiscountRate;
					product.UpdatedAt = DateTime.UtcNow;

					if (!string.IsNullOrEmpty(imagePath))
					{
						product.ImagePath = imagePath;
					}

					await _productRepository.UpdateAsync(product);
					return Result<ProductDto>.Success(MapToDto(product));
				}
				catch (Exception ex)
				{
					return Result<ProductDto>.Failure($"Error updating product: {ex.Message}");
				}
			}

			public async Task<Result<bool>> DeleteProductAsync(int id)
			{
				try
				{
					var product = await _productRepository.GetByIdAsync(id);
					if (product == null)
						return Result<bool>.Failure("Product not found");

					await _productRepository.DeleteAsync(product);
					return Result<bool>.Success(true);
				}
				catch (Exception ex)
				{
					return Result<bool>.Failure($"Error deleting product: {ex.Message}");
				}
			}

			private async Task<string> GenerateUniqueProductCodeAsync()
			{
				var products = await _productRepository.GetAllAsync();
				var lastProduct = products.OrderByDescending(p => p.Id).FirstOrDefault();

				var newCodeNumber = 1;
				if (lastProduct != null && lastProduct.ProductCode.StartsWith("P"))
				{
					if (int.TryParse(lastProduct.ProductCode.Substring(1), out int lastNumber))
					{
						newCodeNumber = lastNumber + 1;
					}
				}

				return $"P{newCodeNumber:D3}";
			}

			private ProductDto MapToDto(Product product)
			{
				return new ProductDto
				{
					Id = product.Id,
					Category = product.Category,
					ProductCode = product.ProductCode,
					Name = product.Name,
					ImagePath = product.ImagePath,
					Price = product.Price,
					MinimumQuantity = product.MinimumQuantity,
					DiscountRate = product.DiscountRate,
					// FinalPrice is calculated in the DTO getter
				};
			}
		}
	}
