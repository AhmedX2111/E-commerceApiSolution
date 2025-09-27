using Ecommerce.Domainn.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Entities
{
	public class User : BaseEntity
	{
		[Required]
		[MaxLength(50)]
		public string UserName { get; set; } = string.Empty;

		[Required]
		[MaxLength(255)]
		public string Password { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[MaxLength(100)]
		public string EmailAddress { get; set; } = string.Empty;

		public DateTime? LastLoginTime { get; set; }

		public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
	}
}

