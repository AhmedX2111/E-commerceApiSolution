using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.UserDtos
{
	public class RefreshTokenRequest
	{
		public string Token { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
	}
}
