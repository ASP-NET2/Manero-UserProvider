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

public class SaveImageServiceTests
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
        var accountUser = new AccountUserEntity
        {
            AccountId = 1,
            IdentityUserId = "user1",
            FirstName = "John",
            LastName = "Doe"
        };

        context.AccountUser.Add(accountUser);
        context.SaveChanges();
    }

    [Fact]
    public async Task SaveImageUrlAsync_ValidAccountId_ShouldSaveImageUrl()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<SaveImageService>>();
        var accountService = new SaveImageService(context, loggerMock.Object);

        // Act
        var result = await accountService.SaveImageUrlAsync(1, "http://example.com/image.jpg");

        // Assert
        Assert.True(result);
        var updatedAccount = await context.AccountUser.FindAsync(1);
        Assert.NotNull(updatedAccount);
        Assert.Equal("http://example.com/image.jpg", updatedAccount.ImageUrl);
    }

    [Fact]
    public async Task SaveImageUrlAsync_InvalidAccountId_ShouldReturnFalse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<SaveImageService>>();
        var accountService = new SaveImageService(context, loggerMock.Object);

        // Act
        var result = await accountService.SaveImageUrlAsync(99, "http://example.com/image.jpg");

        // Assert
        Assert.False(result);
    }
}
