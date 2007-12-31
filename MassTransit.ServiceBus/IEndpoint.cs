using System;

namespace MassTransit.ServiceBus
{
    public interface IEndpoint
    {
		void Send(IEnvelope e);

		event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

		string Address { get; }

        IEndpoint Poison { get; }
    }
}