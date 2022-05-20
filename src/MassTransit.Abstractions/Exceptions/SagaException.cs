namespace MassTransit
{
    using System;
    using System.Linq.Expressions;


    [Serializable]
    public class SagaException :
        MassTransitException
    {
        protected SagaException()
        {
        }

        public SagaException(string message, Type sagaType, Guid correlationId)
            : base(FormatMessage(sagaType, correlationId, message))
        {
            SagaType = sagaType;
            CorrelationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Guid correlationId, Exception innerException)
            : base(FormatMessage(sagaType, correlationId, message), innerException)
        {
            SagaType = sagaType;
            CorrelationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId)
            : base(FormatMessage(sagaType, correlationId, messageType, message))
        {
            SagaType = sagaType;
            MessageType = messageType;
            CorrelationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression)
            : base($"{sagaType.FullName} {message}({messageType.FullName}) - {findExpression}")
        {
            SagaType = sagaType;
            MessageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Expression findExpression, Exception innerException)
            : base($"{sagaType.FullName} {message}({messageType.FullName}) - {findExpression}", innerException)
        {
            SagaType = sagaType;
            MessageType = messageType;
        }

        public SagaException(string message, Type sagaType, Type messageType, Guid correlationId, Exception innerException)
            : base(FormatMessage(sagaType, correlationId, messageType, message), innerException)
        {
            SagaType = sagaType;
            MessageType = messageType;
            CorrelationId = correlationId;
        }

        public SagaException(string message, Type sagaType, Type messageType)
            : base(FormatMessage(sagaType, messageType, message))
        {
            SagaType = sagaType;
            MessageType = messageType;
            CorrelationId = Guid.Empty;
        }

        public Type? SagaType { get; }

        public Type? MessageType { get; }

        public Guid? CorrelationId { get; }

        static string FormatMessage(Type sagaType, Type messageType, string message)
        {
            return $"{TypeCache.GetShortName(sagaType)} Saga exception on receipt of {TypeCache.GetShortName(messageType)}: {message}";
        }

        static string FormatMessage(Type sagaType, Guid correlationId, string message)
        {
            return $"{TypeCache.GetShortName(sagaType)}({correlationId}) Saga exception: {message}";
        }

        static string FormatMessage(Type sagaType, Guid correlationId, Type messageType, string message)
        {
            return
                $"{TypeCache.GetShortName(sagaType)}({correlationId}) Saga exception on receipt of {TypeCache.GetShortName(messageType)}: {message}";
        }
    }
}
