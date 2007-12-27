using System;

namespace MassTransit.ServiceBus
{
    public interface ITransport
    {
    	void Send(IEnvelope e);

        event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

        string Address { get; }
    }
}