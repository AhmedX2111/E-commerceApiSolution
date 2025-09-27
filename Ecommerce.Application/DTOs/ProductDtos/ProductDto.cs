using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.ProductDtos
{
	public class ProductDto
	{
		public int Id { get; set; }
		public string Category { get; set; } = string.Empty;
		public string ProductCode { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string ImagePath { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int MinimumQuantity { get; set; }
		public decimal DiscountRate { get; set; }
		public decimal FinalPrice => Price * (1 - DiscountRate / 100);
	}
}
