namespace MassTransit.RedisIntegration
{
    using System;


    [Serializable]
    public class RedisSagaConcurrencyException :
        SagaException
    {
        public RedisSagaConcurrencyException()
        {
        }

        public RedisSagaConcurrencyException(string message, Type sagaType, Type messageType, Guid correlationId)
            : base(message, sagaType, messageType, correlationId)
        {
        }
    }
}
