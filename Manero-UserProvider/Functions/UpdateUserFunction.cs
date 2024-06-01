using Manero_UserProvider.Models;
using Manero_UserProvider.Services;
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
        private readonly UpdateService _service;

        public UpdateUserFunction(ILogger<UpdateUserFunction> logger, UpdateService service)
        {
            _logger = logger;
            _service = service;
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
                    await response.WriteStringAsync("Invalid profile data");
                    return response;
                }

                var result = await _service.UpdateUserAsync(id, profile);

                if (result == null)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    await response.WriteStringAsync($"User with ID {id} not found.");
                    return response;
                }

                _logger.LogInformation($"Profile for user ID: {id} successfully updated.");
                response.StatusCode = System.Net.HttpStatusCode.OK;
                await response.WriteAsJsonAsync(result);
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
