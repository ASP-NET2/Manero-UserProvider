using Manero_UserProvider.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProviderLibrary.Data.Context;
using UserProviderLibrary.Data.Entities;

namespace UserProviderTests;

public class GetAllServiceTests
{
    private DataContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        SeedDatabase(context);
        return context;
    }

    private void SeedDatabase(DataContext context)
    {
        var accountUsers = new List<AccountUserEntity>
            {
                new AccountUserEntity
                {
                    AccountId = 1,
                    IdentityUserId = "user1",
                    FirstName = "John",
                    LastName = "Doe"
                },
                new AccountUserEntity
                {
                    AccountId = 2,
                    IdentityUserId = "user2",
                    FirstName = "Jane",
                    LastName = "Smith"
                }
            };

        context.AccountUser.AddRange(accountUsers);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetAllService>>();
        var accountService = new GetAllService(context, loggerMock.Object);

        // Act
        var result = await accountService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.IdentityUserId == "user1" && u.FirstName == "John" && u.LastName == "Doe");
        Assert.Contains(result, u => u.IdentityUserId == "user2" && u.FirstName == "Jane" && u.LastName == "Smith");
    }
}
