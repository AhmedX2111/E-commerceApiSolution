using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Ecommerce.Domainn.Entities;
using Ecommerce.Domainn.Interfaces.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Test.UnitTests.Services
{
	public class ProductServiceTests
	{
		private readonly Mock<IProductRepository> _productRepositoryMock;
		private readonly ProductService _productService;

		public ProductServiceTests()
		{
			_productRepositoryMock = new Mock<IProductRepository>();
			_productService = new ProductService(_productRepositoryMock.Object);
		}

		[Fact]
		public async Task GetAllProductsAsync_ReturnsProducts()
		{
			// Arrange
			var products = new List<Product>
			{
				new Product { Id = 1, Name = "Product 1", Price = 100 },
				new Product { Id = 2, Name = "Product 2", Price = 200 }
			};

			_productRepositoryMock.Setup(x => x.GetAllAsync())
				.ReturnsAsync(products);

			// Act
			var result = await _productService.GetAllProductsAsync();

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(2, result.Data.Count());
		}

		[Fact]
		public async Task CreateProductAsync_WithValidData_CreatesProduct()
		{
			// Arrange
			var request = new CreateProductRequest
			{
				Name = "New Product",
				Category = "Electronics",
				Price = 100,
				MinimumQuantity = 1,
				DiscountRate = 10
			};

			var product = new Product
			{
				Id = 1,
				Name = request.Name,
				Category = request.Category,
				Price = request.Price,
				MinimumQuantity = request.MinimumQuantity,
				DiscountRate = request.DiscountRate,
				ProductCode = "P001"
			};

			_productRepositoryMock.Setup(x => x.ProductCodeExistsAsync(It.IsAny<string>()))
				.ReturnsAsync(false);
			_productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>()))
				.ReturnsAsync(product);

			// Act
			var result = await _productService.CreateProductAsync(request, "");

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal("New Product", result.Data.Name);
			Assert.Equal(90, result.Data.FinalPrice); // 10% discount
		}

		[Fact]
		public async Task CreateProductAsync_WithDuplicateProductCode_ReturnsFailure()
		{
			// Arrange
			var request = new CreateProductRequest
			{
				Name = "New Product",
				Category = "Electronics",
				Price = 100,
				MinimumQuantity = 1
			};

			_productRepositoryMock.Setup(x => x.ProductCodeExistsAsync(It.IsAny<string>()))
				.ReturnsAsync(true);

			// Act
			var result = await _productService.CreateProductAsync(request, "");

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Contains("Product code already exists", result.Error);
		}
	}
}
