namespace MassTransit
{
    using System;
    using System.Linq.Expressions;
    using Metadata;


    [Serializable]
    public class SagaException :
        MassTransitException
    {
        readonly Guid _correlationId;
        readonly Type _messageType;
        readonly Type _sagaType;

        protected SagaException()
        {
        }

        public SagaException(string message, Type sagaType, Guid correlationId)
            : base(FormatMessage(sagaType, correlationId, message))
        {
            _sagaType = sagaType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(FormatMessage(sagaType, correlationId, message), innerException)
        {
            _sagaType = sagaType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId)
            : base(FormatMessage(sagaType, correlationId, messageType, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression)
            : base($"{sagaType.FullName} {message}({messageType.FullName}) - {findExpression}")
        {
            _sagaType = sagaType;
            _messageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression, Exception innerException)
            : base($"{sagaType.FullName} {message}({messageType.FullName}) - {findExpression}", innerException)
        {
            _sagaType = sagaType;
            _messageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId, Exception innerException)
            : base(FormatMessage(sagaType, correlationId, messageType, message), innerException)
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType)
            : base(FormatMessage(sagaType, messageType, message))
        {
            _sagaType = sagaType;
            _messageType = messageType;
            _correlationId = Guid.Empty;
        }

        public Type SagaType => _sagaType;

        public Type MessageType => _messageType;

        public Guid CorrelationId => _correlationId;

        static string FormatMessage(Type sagaType, Type messageType, string message)
        {
            return $"{TypeMetadataCache.GetShortName(sagaType)} Saga exception on receipt of {TypeMetadataCache.GetShortName(messageType)}: {message}";
        }

        static string FormatMessage(Type sagaType, Guid correlationId, string message)
        {
            return $"{TypeMetadataCache.GetShortName(sagaType)}({correlationId}) Saga exception: {message}";
        }

        static string FormatMessage(Type sagaType, Guid correlationId, Type messageType, string message)
        {
            return
                $"{TypeMetadataCache.GetShortName(sagaType)}({correlationId}) Saga exception on receipt of {TypeMetadataCache.GetShortName(messageType)}: {message}";
        }
    }
}
