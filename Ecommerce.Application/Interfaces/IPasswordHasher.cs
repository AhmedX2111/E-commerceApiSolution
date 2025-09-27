using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Interfaces
{
	public interface IPasswordHasher
	{
		string Hash(string password);
		bool Verify(string password, string hashedPassword);
	}
}
