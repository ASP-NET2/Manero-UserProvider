using Manero_UserProvider.Handler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Functions
{
    public class DeleteUserFunction
    {
        private readonly ILogger<DeleteUserFunction> _logger;
        private readonly DataContext _context;
        private readonly ServiceBusHandler _handler;

        public DeleteUserFunction(ILogger<DeleteUserFunction> logger, DataContext context, ServiceBusHandler handler)
        {
            _logger = logger;
            _context = context;
            _handler = handler;
        }

        [Function("DeleteUserFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "DeleteUserFunction/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"Received request to delete user ID: {id}");

            var response = req.CreateResponse();

            try
            {
                var profile = await _context.AccountUser.FirstOrDefaultAsync(u => u.IdentityUserId == id);
                if (profile == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    await response.WriteStringAsync($"User with ID {id} not found.");
                    return response;
                }

                await _handler.SendMessageAsync(profile.AccountId);
                _logger.LogInformation($"User with ID: {id} deleted and message sent to Service Bus.");

                _context.AccountUser.Remove(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User with ID {id} has been deleted");
                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteStringAsync($"User with ID {id} has been deleted");
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
