namespace MassTransit.Initializers
{
    using System;
    using Factories;
    using Metadata;


    public static class MessageFactoryCache<TMessage>
        where TMessage : class
    {
        public static IMessageFactory<TMessage> Factory => Cached.MessageFactory.Value;


        static class Cached
        {
            internal static readonly Lazy<IMessageFactory<TMessage>> MessageFactory = new Lazy<IMessageFactory<TMessage>>(CreateMessageFactory);

            static IMessageFactory<TMessage> CreateMessageFactory()
            {
                if (!MessageTypeCache<TMessage>.IsValidMessageType)
                    throw new ArgumentException(MessageTypeCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

                var implementationType = typeof(TMessage);
                if (typeof(TMessage).IsInterface)
                    implementationType = TypeMetadataCache<TMessage>.ImplementationType;

                Type[] parameterTypes = Type.EmptyTypes;
                if (implementationType.GetConstructor(parameterTypes) == null)
                    throw new ArgumentException("No default constructor available for message type", nameof(TMessage));

                return (IMessageFactory<TMessage>)Activator.CreateInstance(typeof(DynamicMessageFactory<,>).MakeGenericType(typeof(TMessage),
                    implementationType));
            }
        }
    }
}
