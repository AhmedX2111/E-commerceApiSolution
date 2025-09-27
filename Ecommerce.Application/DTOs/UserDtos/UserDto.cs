using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.UserDtos
{
	public class UserDto
	{
		public int Id { get; set; }
		public string UserName { get; set; } = string.Empty;
		public string EmailAddress { get; set; } = string.Empty;
		public DateTime? LastLoginTime { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
