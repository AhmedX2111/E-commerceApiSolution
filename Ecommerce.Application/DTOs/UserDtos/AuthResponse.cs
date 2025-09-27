using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.UserDtos
{
	public class AuthResponse
	{
		public string Token { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public int UserId { get; set; }
		public string EmailAddress { get; set; } = string.Empty;
		public DateTime ExpiresAt { get; set; }
		public string TokenType { get; set; } = "Bearer";
	}
}
