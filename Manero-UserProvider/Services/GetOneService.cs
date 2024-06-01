using Manero_UserProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Services;

public class GetOneService
{
    private readonly DataContext _context;
    private readonly ILogger<GetOneService> _logger;

    public GetOneService(DataContext context, ILogger<GetOneService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProfileModel> GetUserByIdAsync(string userId)
    {
        _logger.LogInformation($"Received request for user ID: {userId}");

        try
        {
            var profile = await _context.AccountUser.FirstOrDefaultAsync(u => u.IdentityUserId == userId);

            if (profile == null)
            {
                _logger.LogWarning($"User with ID {userId} not found.");
                return null;
            }

            return new ProfileModel
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                ImageUrl = profile.ImageUrl,
                Location = profile.Location,
                IdentityUserId = profile.IdentityUserId,
                PhoneNumber = profile.PhoneNumber,
                AccountId = profile.AccountId,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
