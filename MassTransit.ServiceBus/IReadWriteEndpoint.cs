using System;

namespace MassTransit.ServiceBus
{
    public interface IReadWriteEndpoint : 
        IEndpoint
    {
        event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

        bool AcceptEnvelope(string id);

        IEndpoint PoisonEndpoint { get; }
    }
}