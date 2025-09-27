using Ecommerce.Domainn.Entities;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Test.UnitTests.Repositories
{
	public class UserRepositoryTests : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly UserRepository _repository;

		public UserRepositoryTests()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
				.Options;

			_context = new ApplicationDbContext(options);
			_repository = new UserRepository(_context);

			SeedTestData();
		}

		private void SeedTestData()
		{
			var users = new List<User>
			{
				new User { UserName = "user1", EmailAddress = "user1@test.com", Password = "hashed1" },
				new User { UserName = "user2", EmailAddress = "user2@test.com", Password = "hashed2" }
			};

			_context.Users.AddRange(users);
			_context.SaveChanges();
		}

		[Fact]
		public async Task GetByUserNameAsync_WithExistingUser_ReturnsUser()
		{
			// Act
			var result = await _repository.GetByUserNameAsync("user1");

			// Assert
			Assert.NotNull(result);
			Assert.Equal("user1", result.UserName);
		}

		[Fact]
		public async Task GetByUserNameAsync_WithNonExistingUser_ReturnsNull()
		{
			// Act
			var result = await _repository.GetByUserNameAsync("nonexistent");

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task UserNameExistsAsync_WithExistingUserName_ReturnsTrue()
		{
			// Act
			var result = await _repository.UserNameExistsAsync("user1");

			// Assert
			Assert.True(result);
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}
	}
}
