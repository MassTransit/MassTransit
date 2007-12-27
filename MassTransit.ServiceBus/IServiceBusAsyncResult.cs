using System;
using System.Collections.Generic;

namespace MassTransit.ServiceBus
{
    public interface IServiceBusAsyncResult :
        IAsyncResult
    {
        IList<IMessage> Messages { get; }
    }
}