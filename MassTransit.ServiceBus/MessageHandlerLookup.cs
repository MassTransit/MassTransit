using System;
using System.Collections.Generic;

namespace MassTransit.ServiceBus
{
    public class MessageHandlerLookup :
        IMessageHandlerLookup
    {
        private readonly Dictionary<Type, List<Type>> _handlers = new Dictionary<Type, List<Type>>();

        public MessageHandlerLookup(params IMessageEndpoint<IMessage>[] endpoints)
        {
            foreach (IMessageEndpoint<IMessage> handler in endpoints)
            {
                Type t = handler.GetType();

                Type parentType = t.BaseType;
                if (parentType.IsGenericType)
                {
                    Type[] genericArgs = parentType.GetGenericArguments();
                    if (genericArgs.Length != 1)
                        continue;

                    if (!typeof (IMessage).IsAssignableFrom(genericArgs[0]))
                        continue;

                    Type handlerType = typeof (IMessageEndpoint<>).MakeGenericType(genericArgs[0]);
                    if (handlerType.IsAssignableFrom(parentType))
                    {
                        Add(t, genericArgs[0]);
                    }
                }
            }
        }
        
        public void Add(Type messageType, Type handlerType)
        {
            if ( !_handlers.ContainsKey(messageType))
                _handlers.Add(messageType, new List<Type>());

            if(!_handlers[messageType].Contains(handlerType))
                _handlers[messageType].Add(handlerType);
        }

        public void Add<T>(IMessageEndpoint<T> endpoint) where T : IMessage
        {
            Add(typeof(T), endpoint.GetType());
        }

        public IList<Type> Find<T>(T msg) where T : IMessage
        {
            List<Type> result;
            _handlers.TryGetValue(typeof (T), out result);

            if(result == null)
                result = new List<Type>();

            return result;
        }
    }
}