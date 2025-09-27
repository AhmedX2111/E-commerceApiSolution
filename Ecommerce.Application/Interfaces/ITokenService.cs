using Ecommerce.Domainn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Interfaces
{
	public interface ITokenService
	{
		string GenerateJwtToken(User user);
		RefreshToken GenerateRefreshToken();
		string? ValidateJwtToken(string token);
	}
}
