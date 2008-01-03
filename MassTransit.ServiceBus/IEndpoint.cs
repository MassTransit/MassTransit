using System;

namespace MassTransit.ServiceBus
{
    public interface IEndpoint :
        IDisposable
    {
		void Send(IEnvelope e);

		event EventHandler<EnvelopeReceivedEventArgs> EnvelopeReceived;

        void AcceptEnvelope(string id);

		string Address { get; }

        IEndpoint Poison { get; }
    }
}