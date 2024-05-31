using Manero_UserProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UserProviderLibrary.Data.Context;

namespace Manero_UserProvider.Functions
{
    public class SaveImageFunction
    {
        private readonly ILogger<SaveImageFunction> _logger;
        private readonly DataContext _context;

        public SaveImageFunction(ILogger<SaveImageFunction> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("SaveImageFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "save-image-url")] HttpRequestData req, FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SaveImageUrl");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SaveImageUrlRequest>(requestBody);

            var account = await _context.AccountUser.FindAsync(data!.AccountId);
            if (account == null)
            {
                var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("Account not found.");
                return notFoundResponse;
            }

            account.ImageUrl = data.ImageUrl;
            _context.AccountUser.Update(account);
            await _context.SaveChangesAsync();

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync("Image url saved to database");

            return response;
        }
    }
}
