namespace MassTransit.Initializers
{
    using System;
    using System.Threading;
    using Contexts;
    using Factories;
    using GreenPipes.Internals.Extensions;
    using Util;


    public static class MessageFactoryCache<TMessage>
        where TMessage : class
    {
        public static IMessageFactory<TMessage> Factory => Cached.MessageFactory.Value;

        public static TMessage CreateMessage()
        {
            return Cached.MessageFactory.Value.Create(new BaseInitializeContext(CancellationToken.None)).Message;
        }


        static class Cached
        {
            internal static readonly Lazy<IMessageFactory<TMessage>> MessageFactory = new Lazy<IMessageFactory<TMessage>>(CreateMessageFactory);

            static IMessageFactory<TMessage> CreateMessageFactory()
            {
                if (!TypeMetadataCache<TMessage>.IsValidMessageType)
                    throw new ArgumentException(TypeMetadataCache<TMessage>.InvalidMessageTypeReason, nameof(TMessage));

                var implementationType = typeof(TMessage);
                if (typeof(TMessage).IsInterface)
                    implementationType = TypeCache.GetImplementationType(typeof(TMessage));

                return (IMessageFactory<TMessage>)Activator.CreateInstance(typeof(DynamicMessageFactory<,>).MakeGenericType(typeof(TMessage),
                    implementationType));
            }
        }
    }
}
