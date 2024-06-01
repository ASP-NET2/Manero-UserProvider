using Manero_UserProvider.Models;
using Manero_UserProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Functions
{
    public class GetUserFunction
    {
        private readonly ILogger<GetUserFunction> _logger;
        private readonly GetOneService _service;

        public GetUserFunction(ILogger<GetUserFunction> logger, GetOneService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function("GetUserFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUserFunction/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"Received request for user ID: {id}");
            var response = req.CreateResponse();
            try
            {
                var result = await _service.GetUserByIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning($"User with ID {id} not found.");
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    await response.WriteStringAsync($"User with ID {id} not found.");
                    return response;
                }

                _logger.LogInformation($"Returning profile for user ID: {id}");
                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteAsJsonAsync(result);
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
