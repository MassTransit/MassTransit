using System;

namespace MassTransit.ServiceBus
{
    public interface IMessageSender :
        IDisposable
    {
        void Send(IEnvelope e);
    }
}