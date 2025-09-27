using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Application.DTOs.UserDtos;
using Ecommerce.Domainn.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Test.IntegrationTests
{
	[Collection("Database collection")]
	public class ProductsControllerIntegrationTests : IAsyncLifetime
	{
		private readonly WebApplicationFactory<Program> _factory;
		private readonly HttpClient _client;
		private readonly ApplicationDbContext _context;
		private string _authToken = string.Empty;

		public ProductsControllerIntegrationTests(IntegrationTestFixture fixture)
		{
			_factory = fixture.Factory;
			_client = fixture.Client;
			_context = fixture.Context;
		}

		public async Task InitializeAsync()
		{
			// Seed test data and get auth token
			await SeedTestData();
			_authToken = await GetAuthToken();
		}

		public Task DisposeAsync() => Task.CompletedTask;

		private async Task SeedTestData()
		{
			// Clear existing data
			_context.Products.RemoveRange(_context.Products);
			_context.Users.RemoveRange(_context.Users);
			await _context.SaveChangesAsync();

			// Add test user
			var user = new User
			{
				UserName = "testuser",
				EmailAddress = "test@test.com",
				Password = BCrypt.Net.BCrypt.HashPassword("password123")
			};
			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			// Add test products
			var products = new List<Product>
			{
				new Product { Category = "Electronics", ProductCode = "P001", Name = "Test Product 1", Price = 100, MinimumQuantity = 1 },
				new Product { Category = "Books", ProductCode = "P002", Name = "Test Product 2", Price = 50, MinimumQuantity = 1 }
			};
			_context.Products.AddRange(products);
			await _context.SaveChangesAsync();
		}

		private async Task<string> GetAuthToken()
		{
			var loginRequest = new LoginRequest { UserName = "testuser", Password = "password123" };
			var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
			response.EnsureSuccessStatusCode();

			var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
			return authResponse!.Token;
		}

		[Fact]
		public async Task GetProducts_WithValidToken_ReturnsProducts()
		{
			// Arrange
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

			// Act
			var response = await _client.GetAsync("/api/products");

			// Assert
			response.EnsureSuccessStatusCode();
			var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
			Assert.NotNull(products);
			Assert.Equal(2, products.Count);
		}

		[Fact]
		public async Task GetProducts_WithoutToken_ReturnsUnauthorized()
		{
			// Arrange
			_client.DefaultRequestHeaders.Authorization = null;

			// Act
			var response = await _client.GetAsync("/api/products");

			// Assert
			Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
		}

		[Fact]
		public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
		{
			// Arrange
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

			var createRequest = new CreateProductRequest
			{
				Category = "Electronics",
				Name = "New Product",
				Price = 200,
				MinimumQuantity = 1,
				DiscountRate = 10
			};

			// Act
			var response = await _client.PostAsJsonAsync("/api/products", createRequest);

			// Assert
			response.EnsureSuccessStatusCode();
			var product = await response.Content.ReadFromJsonAsync<ProductDto>();
			Assert.NotNull(product);
			Assert.Equal("New Product", product.Name);
			Assert.Equal(180, product.FinalPrice); // 200 - 10% discount
		}
	}
}
