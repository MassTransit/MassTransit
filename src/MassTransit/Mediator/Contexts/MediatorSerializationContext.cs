namespace MassTransit.Mediator.Contexts
{
    using System;
    using System.Collections.Generic;
    using Serialization;
    using Util;


    public class MediatorSerializationContext<TMessage> :
        BaseSerializerContext
        where TMessage : class
    {
        readonly TMessage _message;

        public MediatorSerializationContext(IObjectDeserializer deserializer, MessageContext context, TMessage message, string[] supportedMessageTypes)
            : base(deserializer, context, supportedMessageTypes)
        {
            _message = message;
        }

        public override bool TryGetMessage<T>(out T message)
            where T : class
        {
            if (_message is T msg)
            {
                message = msg;
                return true;
            }

            message = null;
            return false;
        }

        public override bool TryGetMessage(Type messageType, out object message)
        {
            if (messageType.IsAssignableFrom(typeof(TMessage)))
            {
                message = _message;
                return true;
            }

            message = null;
            return false;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            throw new NotImplementedByDesignException();
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            throw new NotImplementedByDesignException();
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            throw new NotImplementedByDesignException();
        }

        public override Dictionary<string, object> ToDictionary<T>(T message)
            where T : class
        {
            return ConvertObject.ToDictionary(message);
        }
    }
}
