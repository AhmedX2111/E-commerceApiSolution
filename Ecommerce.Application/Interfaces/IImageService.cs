using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Interfaces
{
	public interface IImageService
	{
		Task<string> SaveImageAsync(IFormFile imageFile, string webRootPath);
		bool DeleteImage(string imagePath, string webRootPath);
		Task<List<string>> SaveMultipleImagesAsync(IEnumerable<IFormFile> imageFiles, string webRootPath);
		bool ImageExists(string imagePath, string webRootPath);
		string GetImageMimeType(string fileName);
		Task<byte[]> GetImageBytesAsync(string imagePath, string webRootPath);
		Task<string> UpdateImageAsync(string oldImagePath, IFormFile newImageFile, string webRootPath);
		void CleanUpOrphanedImages(List<string> usedImagePaths, string webRootPath);
	}
  
}
