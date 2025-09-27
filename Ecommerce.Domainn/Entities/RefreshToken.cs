using Ecommerce.Domainn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Entities
{
	public class RefreshToken : BaseEntity
	{
		public string Token { get; set; } = string.Empty;
		public DateTime Expires { get; set; }
		public DateTime Created { get; set; } = DateTime.UtcNow;
		public DateTime? Revoked { get; set; }
		public string? ReplacedByToken { get; set; }
		public bool IsExpired => DateTime.UtcNow >= Expires;
		public bool IsActive => Revoked == null && !IsExpired;

		public int UserId { get; set; }
		public virtual User User { get; set; } = null!;
	}
}
