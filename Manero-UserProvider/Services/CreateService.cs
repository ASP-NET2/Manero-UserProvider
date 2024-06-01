using Manero_UserProvider.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProviderLibrary.Data.Context;
using UserProviderLibrary.Data.Entities;

namespace Manero_UserProvider.Services
{
    public class CreateService
    {
        private readonly DataContext _context;
        private readonly ILogger<CreateService> _logger;

        public CreateService(DataContext context, ILogger<CreateService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<AccountUserEntity> CreateUserAsync(AccountUserModel data)
        {
            _logger.LogInformation("Received request to create a new user");

            try
            {
                if (data == null)
                {
                    _logger.LogWarning("Invalid user data");
                    return null;
                }

                var accountUser = new AccountUserEntity
                {
                    IdentityUserId = data.IdentityUserId,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                };

                _context.AccountUser.Add(accountUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully");
                return accountUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
