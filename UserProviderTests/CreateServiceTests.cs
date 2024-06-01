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

namespace UserProviderTests
{
    public class CreateServiceTests
    {
        private DataContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);
            return context;
        }

        [Fact]
        public async Task CreateUserAsync_ValidData_ShouldCreateUser()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var loggerMock = new Mock<ILogger<CreateService>>();
            var accountService = new CreateService(context, loggerMock.Object);

            var userModel = new AccountUserModel
            {
                IdentityUserId = "user1",
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var result = await accountService.CreateUserAsync(userModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user1", result.IdentityUserId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public async Task CreateUserAsync_InvalidData_ShouldReturnNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var loggerMock = new Mock<ILogger<CreateService>>();
            var accountService = new CreateService(context, loggerMock.Object);

            // Act
            var result = await accountService.CreateUserAsync(null);

            // Assert
            Assert.Null(result);
        }
    }
}
