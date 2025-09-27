using Ecommerce.Domainn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Entities
{
	public class Product : BaseEntity
	{
		public string Category { get; set; } = string.Empty;
		public string ProductCode { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string ImagePath { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public int MinimumQuantity { get; set; }
		public decimal DiscountRate { get; set; }
	}
}
