using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Manero_UserProvider.Handler;

public class ServiceBusHandler(ServiceBusSender sender)
{
    private readonly ServiceBusSender _sender = sender;

    public async Task SendMessageAsync(int accountId)
    {
        var jsonMessage = JsonConvert.SerializeObject(accountId);
        var message = new ServiceBusMessage(jsonMessage)
        {
            ContentType = "application/json"
        };

        await _sender.SendMessageAsync(message);

    }
}
