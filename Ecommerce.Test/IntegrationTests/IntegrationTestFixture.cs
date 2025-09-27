using Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ecommerce.Test.IntegrationTests
{
	public class IntegrationTestFixture : IAsyncLifetime
	{
		public WebApplicationFactory<Program> Factory { get; private set; } = null!;
		public HttpClient Client { get; private set; } = null!;
		public ApplicationDbContext Context { get; private set; } = null!;

		public async Task InitializeAsync()
		{
			Factory = new WebApplicationFactory<Program>()
				.WithWebHostBuilder(builder =>
				{
					builder.ConfigureTestServices(services =>
					{
						// Replace database with in-memory for testing
						var descriptor = services.SingleOrDefault(
							d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

						if (descriptor != null)
							services.Remove(descriptor);

						services.AddDbContext<ApplicationDbContext>(options =>
						{
							options.UseInMemoryDatabase("TestDatabase");
						});
					});
				});

			Client = Factory.CreateClient();

			var scope = Factory.Services.CreateScope();
			Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			await Context.Database.EnsureCreatedAsync();
		}

		public async Task DisposeAsync()
		{
			await Context.Database.EnsureDeletedAsync();
			Context.Dispose();
			Client.Dispose();
			Factory.Dispose();
		}
	}

	[CollectionDefinition("Database collection")]
	public class DatabaseCollection : ICollectionFixture<IntegrationTestFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}
}
