using Manero_UserProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Functions
{
    public class GetAllFunction
    {
        private readonly ILogger<GetAllFunction> _logger;
        private readonly DataContext _context;

        public GetAllFunction(ILogger<GetAllFunction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetAllFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetAllFunction")] HttpRequestData req)
        {
            _logger.LogInformation("Received request to get all users");

            var response = req.CreateResponse();

            try
            {
                var profiles = await _context.AccountUser.Select(u => new ProfileModel
                {
                    IdentityUserId = u.IdentityUserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                }).ToListAsync();
                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteAsJsonAsync(profiles);
                return response;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                await response.WriteStringAsync(ex.Message);
                return response;
            }
        }
    }
}
