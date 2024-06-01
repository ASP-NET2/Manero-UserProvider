using Manero_UserProvider.Models;
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

public class UpdateServiceTests
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
            LastName = "Doe",
            ImageUrl = "http://example.com/image1.jpg",
            Location = "Location1",
            PhoneNumber = "123456789"
        };

        context.AccountUser.Add(accountUser);
        context.SaveChanges();
    }

    [Fact]
    public async Task UpdateUserAsync_ValidUserId_ShouldUpdateUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<UpdateService>>();
        var accountService = new UpdateService(context, loggerMock.Object);

        var updatedProfile = new ProfileModel
        {
            FirstName = "Jane",
            LastName = "Smith",
            ImageUrl = "http://example.com/image2.jpg",
            Location = "Location2",
            PhoneNumber = "987654321"
        };

        // Act
        var result = await accountService.UpdateUserAsync("user1", updatedProfile);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("http://example.com/image2.jpg", result.ImageUrl);
        Assert.Equal("Location2", result.Location);
        Assert.Equal("987654321", result.PhoneNumber);

        var updatedUser = await context.AccountUser.FirstOrDefaultAsync(u => u.IdentityUserId == "user1");
        Assert.NotNull(updatedUser);
        Assert.Equal("Jane", updatedUser.FirstName);
        Assert.Equal("Smith", updatedUser.LastName);
        Assert.Equal("http://example.com/image2.jpg", updatedUser.ImageUrl);
        Assert.Equal("Location2", updatedUser.Location);
        Assert.Equal("987654321", updatedUser.PhoneNumber);
    }

    [Fact]
    public async Task UpdateUserAsync_InvalidUserId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var loggerMock = new Mock<ILogger<UpdateService>>();
        var accountService = new UpdateService(context, loggerMock.Object);

        var updatedProfile = new ProfileModel
        {
            FirstName = "Jane",
            LastName = "Smith",
            ImageUrl = "http://example.com/image2.jpg",
            Location = "Location2",
            PhoneNumber = "987654321"
        };

        // Act
        var result = await accountService.UpdateUserAsync("invalidUser", updatedProfile);

        // Assert
        Assert.Null(result);
    }
}
