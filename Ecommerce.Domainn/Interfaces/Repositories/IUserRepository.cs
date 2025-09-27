using Ecommerce.Domainn.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domainn.Interfaces.Repositories
{
	public interface IUserRepository : IBaseRepository<User>
	{
		Task<User?> GetByUserNameAsync(string userName);
		Task<User?> GetByEmailAsync(string email);
		Task<bool> UserNameExistsAsync(string userName);
		Task<bool> EmailExistsAsync(string email);
	}
}
