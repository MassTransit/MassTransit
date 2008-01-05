using System;

namespace MassTransit.ServiceBus
{
    public interface IReadWriteEndpoint : 
        IEndpoint
    {
        void Subscribe(IEnvelopeConsumer consumer);

        IEndpoint PoisonEndpoint { get; }
    }
}