namespace MassTransit.EventHubIntegration
{
    using System;


    public interface IProducerProvider
    {
        IEventHubProducer GetProducer(Uri address);
    }
}
