using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using System.Text.Json;
using CookieFactory.Shared;

namespace CookieFactory.Receiver
{
    public static class ReceiverFunction
    {
        [FunctionName("ReceiverFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [WebPubSub(Hub = "CookieFactoryEvents", Connection = "WebPubSubConnectionString")] IAsyncCollector<WebPubSubAction> pubsub,
            [Sql("CookieFactoryEvents", "SqlConnectionString")] IAsyncCollector<CookieFactoryEvent> database,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var cloudEvent = JsonSerializer.Deserialize<CloudEvent>(requestBody);
                await pubsub.AddAsync(WebPubSubAction.CreateSendToAllAction(requestBody));
                await pubsub.FlushAsync();

                var cookieFactoryEvent = cloudEvent.Data.Deserialize<CookieFactoryEvent>();
                await database.AddAsync(cookieFactoryEvent);
                await database.FlushAsync();

                return new OkResult();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
