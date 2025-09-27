using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Interfaces
{
	public interface IUserService
	{
		Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
		Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
		Task<Result<AuthResponse>> RefreshTokenAsync(string token, string refreshToken);
		Task<Result<bool>> RevokeTokenAsync(string refreshToken);
		Task<Result<UserDto>> GetUserByIdAsync(int id);
		Task<Result<UserDto>> GetUserByUsernameAsync(string username);
		Task<Result<bool>> CheckUsernameExistsAsync(string username);
		Task<Result<bool>> CheckEmailExistsAsync(string email);
	}
}
