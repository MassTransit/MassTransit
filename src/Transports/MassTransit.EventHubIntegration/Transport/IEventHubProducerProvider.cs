namespace MassTransit.EventHubIntegration
{
    using System;


    public interface IEventHubProducerProvider
    {
        IEventHubProducer GetProducer(Uri address);
    }
}
