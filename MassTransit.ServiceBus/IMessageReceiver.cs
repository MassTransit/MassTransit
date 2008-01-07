using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageReceiver :
        IDisposable
    {
        void Subscribe(IEnvelopeConsumer consumer);
    }
}