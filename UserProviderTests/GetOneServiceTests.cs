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

public class GetOneServiceTests
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
                LastName = "Doe",
                ImageUrl = "http://example.com/image1.jpg",
                Location = "Location1",
                PhoneNumber = "123456789"
            },
            new AccountUserEntity
            {
                AccountId = 2,
                IdentityUserId = "user2",
                FirstName = "Jane",
                LastName = "Smith",
                ImageUrl = "http://example.com/image2.jpg",
                Location = "Location2",
                PhoneNumber = "987654321"
            }
        };

        context.AccountUser.AddRange(accountUsers);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetUserByIdAsync_ValidUserId_ShouldReturnUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetOneService>>();
        var accountService = new GetOneService(context, loggerMock.Object);

        // Act
        var result = await accountService.GetUserByIdAsync("user1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("http://example.com/image1.jpg", result.ImageUrl);
        Assert.Equal("Location1", result.Location);
        Assert.Equal("123456789", result.PhoneNumber);
        Assert.Equal(1, result.AccountId);
    }

    [Fact]
    public async Task GetUserByIdAsync_InvalidUserId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<GetOneService>>();
        var accountService = new GetOneService(context, loggerMock.Object);

        // Act
        var result = await accountService.GetUserByIdAsync("invalidUser");

        // Assert
        Assert.Null(result);
    }
}

