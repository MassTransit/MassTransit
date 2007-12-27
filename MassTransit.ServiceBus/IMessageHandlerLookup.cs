using System;
using System.Collections.Generic;

namespace MassTransit.ServiceBus
{
    public interface IMessageHandlerLookup
    {
        IList<Type> Find<T>(T msg) where T : IMessage;

        void Add<T>(IMessageEndpoint<T> endpoint) where T : IMessage;
    }
}