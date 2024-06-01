using Manero_UserProvider.Models;
using Manero_UserProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserProviderLibrary.Data.Context;
using UserProviderLibrary.Data.Entities;

namespace Manero_UserProvider.Functions
{
    public class CreateUserFunction
    {
        private readonly ILogger<CreateUserFunction> _logger;
        private readonly CreateService _service;

        public CreateUserFunction(ILogger<CreateUserFunction> logger, CreateService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function("CreateUserFunction")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<AccountUserModel>(requestBody);

                if (data == null)
                {
                    return new BadRequestObjectResult("Invalid user data");
                }

                var result = await _service.CreateUserAsync(data);

                if (result == null)
                {
                    return new BadRequestObjectResult("Invalid user data");
                }

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult("Invalid user data");
            }
        }
    }
}
