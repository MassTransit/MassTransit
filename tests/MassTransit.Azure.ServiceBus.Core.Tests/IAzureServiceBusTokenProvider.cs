namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using Microsoft.Azure.ServiceBus.Primitives;


    public interface IAzureServiceBusTokenProvider
    {
        ITokenProvider GetTokenProvider();
    }
}
