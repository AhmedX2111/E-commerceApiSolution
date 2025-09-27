using Ecommerce.Application.Common;
using Ecommerce.Application.DTOs.UserDtos;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domainn.Entities;
using Ecommerce.Domainn.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly ITokenService _tokenService;
		private readonly IPasswordHasher _passwordHasher;

		public UserService(
			IUserRepository userRepository,
			IRefreshTokenRepository refreshTokenRepository,
			ITokenService tokenService,
			IPasswordHasher passwordHasher)
		{
			_userRepository = userRepository;
			_refreshTokenRepository = refreshTokenRepository;
			_tokenService = tokenService;
			_passwordHasher = passwordHasher;
		}

		public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
		{
			try
			{
				// Check if username already exists
				var existingUserByUsername = await _userRepository.GetByUserNameAsync(request.UserName);
				if (existingUserByUsername != null)
				{
					return Result<AuthResponse>.Failure("Username already exists. Please choose a different username.");
				}

				// Check if email already exists
				var existingUserByEmail = await _userRepository.GetByEmailAsync(request.EmailAddress);
				if (existingUserByEmail != null)
				{
					return Result<AuthResponse>.Failure("Email address already exists. Please use a different email.");
				}

				// Validate password strength
				if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
				{
					return Result<AuthResponse>.Failure("Password must be at least 6 characters long.");
				}

				// Validate email format
				if (!IsValidEmail(request.EmailAddress))
				{
					return Result<AuthResponse>.Failure("Invalid email address format.");
				}

				// Validate username format
				if (!IsValidUsername(request.UserName))
				{
					return Result<AuthResponse>.Failure("Username can only contain letters, numbers, and underscores.");
				}

				var user = new User
				{
					UserName = request.UserName.Trim(),
					EmailAddress = request.EmailAddress.Trim().ToLower(),
					Password = _passwordHasher.Hash(request.Password),
					CreatedAt = DateTime.UtcNow
				};

				var createdUser = await _userRepository.AddAsync(user);

				// Generate token for the newly registered user
				var jwtToken = _tokenService.GenerateJwtToken(createdUser);
				var refreshToken = _tokenService.GenerateRefreshToken();

				refreshToken.UserId = createdUser.Id;
				await _refreshTokenRepository.AddAsync(refreshToken);

				return Result<AuthResponse>.Success(new AuthResponse
				{
					Token = jwtToken,
					RefreshToken = refreshToken.Token,
					UserName = createdUser.UserName,
					ExpiresAt = DateTime.UtcNow.AddHours(1),
					UserId = createdUser.Id
				});
			}
			catch (Exception ex)
			{
				// Handle database unique constraint violations
				if (ex.InnerException?.Message.Contains("IX_Users_UserName") == true)
				{
					return Result<AuthResponse>.Failure("Username already exists. Please choose a different username.");
				}
				if (ex.InnerException?.Message.Contains("IX_Users_EmailAddress") == true)
				{
					return Result<AuthResponse>.Failure("Email address already exists. Please use a different email.");
				}

				return Result<AuthResponse>.Failure($"Registration failed: {ex.Message}");
			}
		}

		public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
		{
			try
			{
				var user = await _userRepository.GetByUserNameAsync(request.UserName);
				if (user == null || !_passwordHasher.Verify(request.Password, user.Password))
				{
					return Result<AuthResponse>.Failure("Invalid username or password.");
				}

				user.LastLoginTime = DateTime.UtcNow;
				await _userRepository.UpdateAsync(user);

				var jwtToken = _tokenService.GenerateJwtToken(user);
				var refreshToken = _tokenService.GenerateRefreshToken();

				refreshToken.UserId = user.Id;
				await _refreshTokenRepository.AddAsync(refreshToken);

				return Result<AuthResponse>.Success(new AuthResponse
				{
					Token = jwtToken,
					RefreshToken = refreshToken.Token,
					UserName = user.UserName,
					UserId = user.Id,
					EmailAddress = user.EmailAddress,
					ExpiresAt = DateTime.UtcNow.AddHours(1),
					TokenType = "Bearer"
				});
			}
			catch (Exception ex)
			{
				return Result<AuthResponse>.Failure($"Login failed: {ex.Message}");
			}
		}

		public async Task<Result<AuthResponse>> RefreshTokenAsync(string token, string refreshToken)
		{
			try
			{
				var userName = _tokenService.ValidateJwtToken(token);
				if (string.IsNullOrEmpty(userName))
				{
					return Result<AuthResponse>.Failure("Invalid token.");
				}

				var user = await _userRepository.GetByUserNameAsync(userName);
				if (user == null)
				{
					return Result<AuthResponse>.Failure("User not found.");
				}

				var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
				if (existingRefreshToken == null || !existingRefreshToken.IsActive)
				{
					return Result<AuthResponse>.Failure("Invalid refresh token.");
				}

				var newRefreshToken = _tokenService.GenerateRefreshToken();
				await RevokeRefreshToken(existingRefreshToken, newRefreshToken.Token);

				newRefreshToken.UserId = user.Id;
				await _refreshTokenRepository.AddAsync(newRefreshToken);

				var newJwtToken = _tokenService.GenerateJwtToken(user);

				return Result<AuthResponse>.Success(new AuthResponse
				{
					Token = newJwtToken,
					RefreshToken = newRefreshToken.Token,
					UserName = user.UserName,
					ExpiresAt = DateTime.UtcNow.AddHours(1)
				});
			}
			catch (Exception ex)
			{
				return Result<AuthResponse>.Failure($"Token refresh failed: {ex.Message}");
			}
		}

		public async Task<Result<bool>> RevokeTokenAsync(string refreshToken)
		{
			try
			{
				var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
				if (token == null)
				{
					return Result<bool>.Failure("Refresh token not found.");
				}

				token.Revoked = DateTime.UtcNow;
				await _refreshTokenRepository.UpdateAsync(token);

				return Result<bool>.Success(true);
			}
			catch (Exception ex)
			{
				return Result<bool>.Failure($"Token revocation failed: {ex.Message}");
			}
		}

		public async Task<Result<UserDto>> GetUserByIdAsync(int id)
		{
			try
			{
				var user = await _userRepository.GetByIdAsync(id);
				if (user == null)
				{
					return Result<UserDto>.Failure("User not found.");
				}

				return Result<UserDto>.Success(new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					EmailAddress = user.EmailAddress,
					LastLoginTime = user.LastLoginTime
				});
			}
			catch (Exception ex)
			{
				return Result<UserDto>.Failure($"Error retrieving user: {ex.Message}");
			}
		}

		public async Task<Result<UserDto>> GetUserByUsernameAsync(string username)
		{
			try
			{
				var user = await _userRepository.GetByUserNameAsync(username);
				if (user == null)
				{
					return Result<UserDto>.Failure("User not found.");
				}

				return Result<UserDto>.Success(new UserDto
				{
					Id = user.Id,
					UserName = user.UserName,
					EmailAddress = user.EmailAddress,
					LastLoginTime = user.LastLoginTime
				});
			}
			catch (Exception ex)
			{
				return Result<UserDto>.Failure($"Error retrieving user: {ex.Message}");
			}
		}

		public async Task<Result<bool>> CheckUsernameExistsAsync(string username)
		{
			try
			{
				var user = await _userRepository.GetByUserNameAsync(username);
				return Result<bool>.Success(user != null);
			}
			catch (Exception ex)
			{
				return Result<bool>.Failure($"Error checking username: {ex.Message}");
			}
		}

		public async Task<Result<bool>> CheckEmailExistsAsync(string email)
		{
			try
			{
				var user = await _userRepository.GetByEmailAsync(email);
				return Result<bool>.Success(user != null);
			}
			catch (Exception ex)
			{
				return Result<bool>.Failure($"Error checking email: {ex.Message}");
			}
		}

		private async Task RevokeRefreshToken(RefreshToken token, string replacedByToken = null)
		{
			token.Revoked = DateTime.UtcNow;
			token.ReplacedByToken = replacedByToken;
			await _refreshTokenRepository.UpdateAsync(token);
		}

		private bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		private bool IsValidUsername(string username)
		{
			// Username can only contain letters, numbers, and underscores
			return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$");
		}
	}
}	
