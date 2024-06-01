using Manero_UserProvider.Models;
using Manero_UserProvider.Services;
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
        private readonly SaveImageService _service;

        public SaveImageFunction(ILogger<SaveImageFunction> logger, SaveImageService service)
        {
            _logger = logger;
            _service = service;
        }

        [Function("SaveImageFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "save-image-url")] HttpRequestData req, FunctionContext executionContext)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<SaveImageUrlRequest>(requestBody);

            if (data == null)
            {
                var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid request data.");
                return badRequestResponse;
            }

            var result = await _service.SaveImageUrlAsync(data.AccountId, data.ImageUrl);

            if (!result)
            {
                var notFoundResponse = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("Account not found.");
                return notFoundResponse;
            }

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync("Image URL saved to database");
            return response;
        }

    }
}
