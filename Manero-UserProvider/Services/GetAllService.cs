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

public class GetAllService
{
    private readonly DataContext _context;
    private readonly ILogger<GetAllService> _logger;

    public GetAllService(DataContext context, ILogger<GetAllService> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<List<ProfileModel>> GetAllUsersAsync()
    {
        _logger.LogInformation("Received request to get all users");

        try
        {
            var profiles = await _context.AccountUser.Select(u => new ProfileModel
            {
                IdentityUserId = u.IdentityUserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
            }).ToListAsync();

            return profiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
