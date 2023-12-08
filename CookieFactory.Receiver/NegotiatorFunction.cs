using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;

namespace CookieFactory.Receiver
{
    public static class NegotiatorFunction
    {
        [FunctionName(nameof(NegotiatorFunction))]
        public static WebPubSubConnection Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "negotiate")] HttpRequest req,
            [WebPubSubConnection(Hub = "CookieFactoryEvents", Connection = "WebPubSubConnectionString")] WebPubSubConnection connection)
        {
            return connection;
        }
    }
}
