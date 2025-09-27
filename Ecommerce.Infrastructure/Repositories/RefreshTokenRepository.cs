using Ecommerce.Domainn.Entities;
using Ecommerce.Domainn.Interfaces.Repositories;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
	public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<RefreshToken?> GetByTokenAsync(string token)
		{
			return await _context.RefreshTokens
				.Include(rt => rt.User)
				.FirstOrDefaultAsync(rt => rt.Token == token);
		}

		public async Task RevokeTokenAsync(string token, string replacedByToken = null)
		{
			var refreshToken = await GetByTokenAsync(token);
			if (refreshToken != null)
			{
				refreshToken.Revoked = DateTime.UtcNow;
				refreshToken.ReplacedByToken = replacedByToken;
				await UpdateAsync(refreshToken);
			}
		}

		public async Task RevokeDescendantTokensAsync(RefreshToken token, string newToken)
		{
			if (!string.IsNullOrEmpty(token.ReplacedByToken))
			{
				var childToken = await GetByTokenAsync(token.ReplacedByToken);
				if (childToken != null && childToken.IsActive)
				{
					await RevokeTokenAsync(childToken.Token, newToken);
				}
				else if (childToken != null)
				{
					await RevokeDescendantTokensAsync(childToken, newToken);
				}
			}
		}
	}
}
