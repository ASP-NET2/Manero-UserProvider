using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Services
{
    public class SaveImageService
    {
        private readonly DataContext _context;
        private readonly ILogger<SaveImageService> _logger;

        public SaveImageService(DataContext context, ILogger<SaveImageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> SaveImageUrlAsync(int accountId, string imageUrl)
        {
            _logger.LogInformation($"Received request to save image URL for account ID: {accountId}");

            try
            {
                var account = await _context.AccountUser.FindAsync(accountId);
                if (account == null)
                {
                    _logger.LogWarning($"Account with ID {accountId} not found.");
                    return false;
                }

                account.ImageUrl = imageUrl;
                _context.AccountUser.Update(account);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image URL saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
