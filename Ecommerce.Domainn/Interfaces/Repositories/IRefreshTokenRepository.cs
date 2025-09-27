using Ecommerce.Domainn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Interfaces.Repositories
{
	public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
	{
		Task<RefreshToken?> GetByTokenAsync(string token);
		Task RevokeTokenAsync(string token, string replacedByToken = null);
		Task RevokeDescendantTokensAsync(RefreshToken token, string newToken);
	
	}
}
