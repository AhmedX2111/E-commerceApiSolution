using Ecommerce.Domainn.Common;
using Ecommerce.Domainn.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		// DbSets map our entities to database tables
		public DbSet<User> Users => Set<User>();
		public DbSet<Product> Products => Set<Product>();
		public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

		// ===== Auto timestamps =====
		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			ApplyTimestamps();
			return await base.SaveChangesAsync(cancellationToken);
		}

		public override int SaveChanges()
		{
			ApplyTimestamps();
			return base.SaveChanges();
		}

		private void ApplyTimestamps()
		{
			var entries = ChangeTracker.Entries()
				.Where(e => e.Entity is BaseEntity &&
							(e.State == EntityState.Added || e.State == EntityState.Modified));

			foreach (var entityEntry in entries)
			{
				var entity = (BaseEntity)entityEntry.Entity;

				if (entityEntry.State == EntityState.Added)
				{
					entity.CreatedAt = DateTime.UtcNow;
				}

				entity.UpdatedAt = DateTime.UtcNow;
			}
		}

		// ===== Model configuration =====
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Fix decimal precision warnings
			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Product>()
				.Property(p => p.DiscountRate)
				.HasPrecision(5, 2);

			// Call seed method
			SeedData(modelBuilder);
		}

		// ===== Seed initial data =====
		private void SeedData(ModelBuilder modelBuilder)
		{
			// Seed initial users
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					UserName = "admin",
					EmailAddress = "admin@ecommerce.com",
					Password = "$2a$11$exampleHashedPassword1", // replace with real hash
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				},
				new User
				{
					Id = 2,
					UserName = "testuser",
					EmailAddress = "test@ecommerce.com",
					Password = "$2a$11$exampleHashedPassword2", // replace with real hash
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				}
			);

			// Seed initial products
			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					Id = 1,
					Category = "Electronics",
					ProductCode = "P001",
					Name = "Wireless Mouse",
					ImagePath = "/images/mouse.jpg",
					Price = 29.99m,
					MinimumQuantity = 1,
					DiscountRate = 10.0m,
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				},
				new Product
				{
					Id = 2,
					Category = "Electronics",
					ProductCode = "P002",
					Name = "Mechanical Keyboard",
					ImagePath = "/images/keyboard.jpg",
					Price = 99.99m,
					MinimumQuantity = 1,
					DiscountRate = 15.0m,
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				},
				new Product
				{
					Id = 3,
					Category = "Books",
					ProductCode = "P003",
					Name = "Programming Guide",
					ImagePath = "/images/book.jpg",
					Price = 49.99m,
					MinimumQuantity = 1,
					DiscountRate = 5.0m,
					CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				}
			);
		}
	}

}
