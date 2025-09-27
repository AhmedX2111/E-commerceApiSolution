using Ecommerce.Application.DTOs.UserDtos;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using Ecommerce.Domainn.Entities;
using Ecommerce.Domainn.Interfaces.Repositories;
using Microsoft.AspNet.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using IPasswordHasher = Ecommerce.Application.Interfaces.IPasswordHasher;

namespace Ecommerce.Test.UnitTests.Services
{
	public class UserServiceTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
		private readonly Mock<ITokenService> _tokenServiceMock;
		private readonly Mock<IPasswordHasher> _passwordHasherMock;
		private readonly UserService _userService;

		public UserServiceTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
			_tokenServiceMock = new Mock<ITokenService>();
			_passwordHasherMock = new Mock<IPasswordHasher>();

			_userService = new UserService(
				_userRepositoryMock.Object,
				_refreshTokenRepositoryMock.Object,
				_tokenServiceMock.Object,
				_passwordHasherMock.Object);
		}

		[Fact]
		public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
		{
			// Arrange
			var request = new LoginRequest { UserName = "test", Password = "password" };
			var user = new User { Id = 1, UserName = "test", Password = "hashed" };

			_userRepositoryMock.Setup(x => x.GetByUserNameAsync(request.UserName))
				.ReturnsAsync(user);
			_passwordHasherMock.Setup(x => x.Verify(request.Password, user.Password))
				.Returns(true);
			_tokenServiceMock.Setup(x => x.GenerateJwtToken(user))
				.Returns("jwt-token");

			// Act
			var result = await _userService.LoginAsync(request);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.Data);
			Assert.Equal("jwt-token", result.Data.Token);
		}
}	}
