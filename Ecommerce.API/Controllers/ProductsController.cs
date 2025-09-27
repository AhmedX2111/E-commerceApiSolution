using Ecommerce.Application.DTOs.ProductDtos;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly IImageService _imageService;
		private readonly IWebHostEnvironment _environment;

		public ProductsController(
			IProductService productService,
			IImageService imageService,
			IWebHostEnvironment environment)
		{
			_productService = productService;
			_imageService = imageService;
			_environment = environment;
		}

		[HttpGet]
		[AllowAnonymous] // Allow public access to view products
		public async Task<IActionResult> GetProducts()
		{
			var result = await _productService.GetAllProductsAsync();

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return Ok(result.Data);
		}

		[HttpGet("{id}")]
		[AllowAnonymous] // Allow public access to view individual product
		public async Task<IActionResult> GetProduct(int id)
		{
			var result = await _productService.GetProductByIdAsync(id);

			if (!result.IsSuccess)
				return NotFound(new { message = result.Error });

			return Ok(result.Data);
		}

		[HttpGet("category/{category}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetProductsByCategory(string category)
		{
			var result = await _productService.GetAllProductsAsync();

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			var filteredProducts = result.Data?.Where(p =>
				p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

			return Ok(filteredProducts);
		}

		[HttpPost]
		public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest request, IFormFile? image)
		{
			// Validate model state
			if (!ModelState.IsValid)
			{
				return BadRequest(new { message = "Invalid product data", errors = ModelState.Values.SelectMany(v => v.Errors) });
			}

			string imagePath = string.Empty;

			if (image != null && image.Length > 0)
			{
				try
				{
					var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
					imagePath = await _imageService.SaveImageAsync(image, webRootPath);
				}
				catch (ArgumentException ex)
				{
					return BadRequest(new { message = ex.Message });
				}
				catch (Exception ex)
				{
					return StatusCode(500, new { message = "Error saving image: " + ex.Message });
				}
			}

			var result = await _productService.CreateProductAsync(request, imagePath);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return CreatedAtAction(nameof(GetProduct), new { id = result.Data.Id }, result.Data);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request) // Remove [FromForm] and IFormFile? image
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { message = "Invalid product data", errors = ModelState.Values.SelectMany(v => v.Errors) });
			}

			// Check if product exists
			var existingProduct = await _productService.GetProductByIdAsync(id);
			if (!existingProduct.IsSuccess)
				return NotFound(new { message = existingProduct.Error });

			// Use existing image path since we're not updating image via JSON
			var currentImagePath = existingProduct.Data?.ImagePath;

			var result = await _productService.UpdateProductAsync(id, request, currentImagePath);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return Ok(new
			{
				message = "Product updated successfully",
				product = result.Data
			});
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduct(int id)
		{
			// Get product first to delete associated image
			var productResult = await _productService.GetProductByIdAsync(id);
			if (productResult.IsSuccess && !string.IsNullOrEmpty(productResult.Data?.ImagePath))
			{
				var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
				_imageService.DeleteImage(productResult.Data.ImagePath, webRootPath);
			}

			var result = await _productService.DeleteProductAsync(id);

			if (!result.IsSuccess)
				return NotFound(new { message = result.Error });

			return NoContent();
		}

		[HttpPatch("{id}/image")]
		public async Task<IActionResult> UpdateProductImage(int id, IFormFile image)
		{
			if (image == null || image.Length == 0)
				return BadRequest(new { message = "No image provided" });

			// Check if product exists
			var existingProduct = await _productService.GetProductByIdAsync(id);
			if (!existingProduct.IsSuccess)
				return NotFound(new { message = existingProduct.Error });

			string newImagePath;
			try
			{
				var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

				// Delete old image if exists
				if (!string.IsNullOrEmpty(existingProduct.Data?.ImagePath))
				{
					_imageService.DeleteImage(existingProduct.Data.ImagePath, webRootPath);
				}

				newImagePath = await _imageService.SaveImageAsync(image, webRootPath);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Error updating image: " + ex.Message });
			}

			// Update product with new image path
			var updateRequest = new UpdateProductRequest
			{
				Category = existingProduct.Data.Category,
				Name = existingProduct.Data.Name,
				Price = existingProduct.Data.Price,
				MinimumQuantity = existingProduct.Data.MinimumQuantity,
				DiscountRate = existingProduct.Data.DiscountRate
			};

			var result = await _productService.UpdateProductAsync(id, updateRequest, newImagePath);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return Ok(new { message = "Image updated successfully", imagePath = newImagePath });
		}

		[HttpGet("search")]
		[AllowAnonymous]
		public async Task<IActionResult> SearchProducts([FromQuery] string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
				return BadRequest(new { message = "Search term is required" });

			var result = await _productService.GetAllProductsAsync();

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			var searchResults = result.Data?.Where(p =>
				p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
				p.Category.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
				p.ProductCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

			return Ok(searchResults);
		}

		[HttpGet("categories")]
		[AllowAnonymous]
		public async Task<IActionResult> GetCategories()
		{
			var result = await _productService.GetAllProductsAsync();

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			var categories = result.Data?
				.Select(p => p.Category)
				.Distinct()
				.OrderBy(c => c)
				.ToList();

			return Ok(categories);
		}

		[HttpPost("bulk")]
		public async Task<IActionResult> CreateProducts([FromBody] List<CreateProductRequest> requests)
		{
			if (!ModelState.IsValid || requests == null || !requests.Any())
			{
				return BadRequest(new { message = "Invalid product data" });
			}

			var results = new List<ProductDto>();
			var errors = new List<string>();

			foreach (var request in requests)
			{
				var result = await _productService.CreateProductAsync(request, string.Empty);
				if (result.IsSuccess)
				{
					results.Add(result.Data);
				}
				else
				{
					errors.Add($"Product {request.Name}: {result.Error}");
				}
			}

			if (errors.Any())
			{
				return StatusCode(207, new
				{
					message = "Some products were not created successfully",
					created = results,
					errors = errors
				});
			}

			return Ok(new { message = "All products created successfully", products = results });
		}
	}
}	
