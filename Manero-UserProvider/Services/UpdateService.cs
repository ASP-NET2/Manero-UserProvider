using Manero_UserProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Services;

public class UpdateService
{
    private readonly DataContext _context;
    private readonly ILogger<UpdateService> _logger;

    public UpdateService(DataContext context, ILogger<UpdateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProfileModel> UpdateUserAsync(string userId, ProfileModel profile)
    {
        _logger.LogInformation($"Received request to update user ID: {userId}");

        try
        {
            var user = await _context.AccountUser.FirstOrDefaultAsync(u => u.IdentityUserId == userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found.");
                return null;
            }

            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            user.ImageUrl = profile.ImageUrl;
            user.PhoneNumber = profile.PhoneNumber;
            user.Location = profile.Location;

            _context.AccountUser.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Profile for user ID: {userId} successfully updated.");
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
