namespace MassTransit.Azure.ServiceBus.Core.Hosting
{
    using System;


    public static class AzureServiceBusEndpointUriCreator
    {
        public static Uri Create(string serviceBusNamespace, string entityPath = null, string azureEndPoint = "servicebus.windows.net")
        {
            var endpoint = $"sb://{serviceBusNamespace}.{azureEndPoint}/{entityPath}";

            return new Uri(endpoint);
        }
    }
}
