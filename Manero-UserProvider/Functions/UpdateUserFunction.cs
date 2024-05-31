using Manero_UserProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Functions
{
    public class UpdateUserFunction
    {
        private readonly ILogger<UpdateUserFunction> _logger;
        private readonly DataContext _context;

        public UpdateUserFunction(ILogger<UpdateUserFunction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("UpdateUserFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "UpdateUserFunction/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"Received request to update user ID: {id}");

            var response = req.CreateResponse();
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var profile = JsonConvert.DeserializeObject<ProfileModel>(requestBody);

                if (profile == null)
                {
                    _logger.LogWarning($"Invalid profile data for user {id}");
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    await response.WriteStringAsync("Invalid prfile data");
                    return response;
                }

                var user = await _context.AccountUser.FirstOrDefaultAsync(u => u.IdentityUserId == id);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    await response.WriteStringAsync($"User with ID {id} not found.");
                    return response;
                }
                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;
                user.ImageUrl = profile.ImageUrl;
                user.PhoneNumber = profile.PhoneNumber;
                user.Location = profile.Location;

                _context.AccountUser.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Profile for user ID: {id} successfully updated.");
                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteAsJsonAsync(profile);
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                await response.WriteStringAsync($"{ex.Message}");
                return response;
            }
        }

    }
}
