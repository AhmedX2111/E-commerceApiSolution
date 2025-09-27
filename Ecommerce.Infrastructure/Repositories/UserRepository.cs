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
	public class UserRepository : BaseRepository<User>, IUserRepository
	{
		public UserRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<User?> GetByUserNameAsync(string userName)
		{
			return await _context.Users
				.FirstOrDefaultAsync(u => u.UserName == userName);
		}

		public async Task<User?> GetByEmailAsync(string email)
		{
			return await _context.Users
				.FirstOrDefaultAsync(u => u.EmailAddress == email);
		}

		public async Task<bool> UserNameExistsAsync(string userName)
		{
			return await _context.Users
				.AnyAsync(u => u.UserName == userName);
		}

		public async Task<bool> EmailExistsAsync(string email)
		{
			return await _context.Users
				.AnyAsync(u => u.EmailAddress == email);
		}
	}
}
