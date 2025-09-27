using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Services
{
	public class ImageService : IImageService
	{
		public async Task<string> SaveImageAsync(IFormFile imageFile, string webRootPath)
		{
			if (imageFile == null || imageFile.Length == 0)
				return string.Empty;

			// Validate file type
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
			var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

			if (string.IsNullOrEmpty(fileExtension) || !allowedExtensions.Contains(fileExtension))
			{
				throw new ArgumentException("Invalid file type. Allowed types: JPG, JPEG, PNG, GIF, BMP, WEBP");
			}

			// Validate file size (max 10MB)
			const int maxFileSize = 10 * 1024 * 1024; // 10MB
			if (imageFile.Length > maxFileSize)
			{
				throw new ArgumentException($"File size too large. Maximum size is {maxFileSize / (1024 * 1024)}MB.");
			}

			// Create images directory if it doesn't exist
			var imagesFolder = Path.Combine(webRootPath, "images");
			if (!Directory.Exists(imagesFolder))
			{
				Directory.CreateDirectory(imagesFolder);
			}

			// Generate unique filename
			var fileName = $"{Guid.NewGuid()}{fileExtension}";
			var filePath = Path.Combine(imagesFolder, fileName);

			// Save the file
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await imageFile.CopyToAsync(stream);
			}

			// Return the relative path
			return $"/images/{fileName}";
		}

		public bool DeleteImage(string imagePath, string webRootPath)
		{
			if (string.IsNullOrEmpty(imagePath))
				return false;

			try
			{
				// Remove leading slash if present and combine with web root path
				var relativePath = imagePath.TrimStart('/');
				var fullPath = Path.Combine(webRootPath, relativePath);

				// Check if file exists and delete it
				if (File.Exists(fullPath))
				{
					File.Delete(fullPath);
					return true;
				}

				// File doesn't exist
				return false;
			}
			catch (Exception ex)
			{
				// Log the exception (you might want to use ILogger here)
				Console.WriteLine($"Error deleting image: {ex.Message}");
				return false;
			}
		}

		public async Task<List<string>> SaveMultipleImagesAsync(IEnumerable<IFormFile> imageFiles, string webRootPath)
		{
			var savedPaths = new List<string>();

			if (imageFiles == null || !imageFiles.Any())
				return savedPaths;

			foreach (var imageFile in imageFiles)
			{
				if (imageFile != null && imageFile.Length > 0)
				{
					var savedPath = await SaveImageAsync(imageFile, webRootPath);
					if (!string.IsNullOrEmpty(savedPath))
					{
						savedPaths.Add(savedPath);
					}
				}
			}

			return savedPaths;
		}

		public bool ImageExists(string imagePath, string webRootPath)
		{
			if (string.IsNullOrEmpty(imagePath))
				return false;

			var relativePath = imagePath.TrimStart('/');
			var fullPath = Path.Combine(webRootPath, relativePath);
			return File.Exists(fullPath);
		}

		public string GetImageMimeType(string fileName)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();
			return extension switch
			{
				".jpg" or ".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".bmp" => "image/bmp",
				".webp" => "image/webp",
				_ => "application/octet-stream"
			};
		}

		public async Task<byte[]> GetImageBytesAsync(string imagePath, string webRootPath)
		{
			if (string.IsNullOrEmpty(imagePath))
				return Array.Empty<byte>();

			var relativePath = imagePath.TrimStart('/');
			var fullPath = Path.Combine(webRootPath, relativePath);

			if (!File.Exists(fullPath))
				return Array.Empty<byte>();

			return await File.ReadAllBytesAsync(fullPath);
		}

		public async Task<string> UpdateImageAsync(string oldImagePath, IFormFile newImageFile, string webRootPath)
		{
			// Delete old image if it exists
			if (!string.IsNullOrEmpty(oldImagePath))
			{
				DeleteImage(oldImagePath, webRootPath);
			}

			// Save new image
			return await SaveImageAsync(newImageFile, webRootPath);
		}

		public void CleanUpOrphanedImages(List<string> usedImagePaths, string webRootPath)
		{
			var imagesFolder = Path.Combine(webRootPath, "images");
			if (!Directory.Exists(imagesFolder))
				return;

			var allImageFiles = Directory.GetFiles(imagesFolder)
				.Select(f => $"/images/{Path.GetFileName(f)}")
				.ToList();

			var orphanedImages = allImageFiles.Except(usedImagePaths).ToList();

			foreach (var orphanedImage in orphanedImages)
			{
				DeleteImage(orphanedImage, webRootPath);
			}
		}
	}
}
