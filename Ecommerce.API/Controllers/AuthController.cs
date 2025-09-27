using Ecommerce.Application.DTOs.UserDtos;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { message = "Invalid registration data", errors = ModelState.Values.SelectMany(v => v.Errors) });
			}

			var result = await _userService.RegisterAsync(request);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			// Return the AuthResponse directly which includes the token
			return Ok(new
			{
				message = "User registered successfully",
				user = new
				{
					result.Data.UserId,
					result.Data.UserName,
					result.Data.EmailAddress
				},
				token = result.Data.Token,
				refreshToken = result.Data.RefreshToken,
				expiresAt = result.Data.ExpiresAt,
				tokenType = result.Data.TokenType
			});
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new { message = "Invalid login data" });
			}

			var result = await _userService.LoginAsync(request);

			if (!result.IsSuccess)
				return Unauthorized(new { message = result.Error });

			// Return the AuthResponse directly which includes the token
			return Ok(new
			{
				message = "Login successful",
				user = new
				{
					result.Data.UserId,
					result.Data.UserName,
					result.Data.EmailAddress
				},
				token = result.Data.Token,
				refreshToken = result.Data.RefreshToken,
				expiresAt = result.Data.ExpiresAt,
				tokenType = result.Data.TokenType
			});
		}

		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var result = await _userService.RefreshTokenAsync(request.Token, request.RefreshToken);

			if (!result.IsSuccess)
				return Unauthorized(new { message = result.Error });

			return Ok(new
			{
				message = "Token refreshed successfully",
				token = result.Data.Token,
				refreshToken = result.Data.RefreshToken,
				expiresAt = result.Data.ExpiresAt,
				tokenType = result.Data.TokenType
			});
		}

		[HttpPost("revoke-token")]
		public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
		{
			var result = await _userService.RevokeTokenAsync(request.RefreshToken);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return Ok(new { message = "Token revoked successfully" });
		}

		[HttpGet("check-username/{username}")]
		public async Task<IActionResult> CheckUsernameExists(string username)
		{
			var result = await _userService.CheckUsernameExistsAsync(username);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return Ok(new { exists = result.Data });
		}

		[HttpGet("check-email/{email}")]
		public async Task<IActionResult> CheckEmailExists(string email)
		{
			var result = await _userService.CheckEmailExistsAsync(email);

			if (!result.IsSuccess)
				return BadRequest(new { message = result.Error });

			return Ok(new { exists = result.Data });
		}
	}
}	
