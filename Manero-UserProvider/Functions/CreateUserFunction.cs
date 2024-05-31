using Manero_UserProvider.Models;
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
        private readonly DataContext _context;

        public CreateUserFunction(ILogger<CreateUserFunction> logger)
        {
            _logger = logger;
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

                var accountUser = new AccountUserEntity
                {
                    IdentityUserId = data.IdentityUserId,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                };

                _context.AccountUser.Add(accountUser);
                await _context.SaveChangesAsync();

                return new OkObjectResult(data);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult("Invalid user data");
            }
        }
    }
}
