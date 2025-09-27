using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.UserDtos
{
	public class RegisterRequest
	{
		public string UserName { get; set; } = string.Empty;
		public string EmailAddress { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
}
