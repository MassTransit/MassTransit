namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using RabbitMQ.Client;


    public static class RabbitMqSendContextExtensions
    {
        public static void SetTransportHeader(this RabbitMqSendContext context, string key, object value)
        {
            SetHeader(context.BasicProperties, key, value);
        }

        public static void SetHeader(this IBasicProperties basicProperties, string key, object value)
        {
            if (value == null)
                return;

            basicProperties.Headers ??= new Dictionary<string, object>();

            basicProperties.Headers[key] = value switch
            {
                DateTime dateTime => dateTime.ToString("O"),
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
                _ => value
            };
        }

        /// <summary>
        /// Sets the priority of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static void SetPriority(this SendContext context, byte priority)
        {
            if (!context.TryGetPayload(out RabbitMqSendContext sendContext))
                throw new ArgumentException("The RabbitMqSendContext was not available");

            sendContext.BasicProperties.Priority = priority;
        }

        /// <summary>
        /// Sets the priority of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static bool TrySetPriority(this SendContext context, byte priority)
        {
            if (!context.TryGetPayload(out RabbitMqSendContext sendContext))
                return false;

            sendContext.BasicProperties.Priority = priority;
            return true;
        }

        /// <summary>
        /// Sets whether the send should wait for the ack from the broker, or if it should
        /// return immediately after the message is sent by the client.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="awaitAck"></param>
        public static void SetAwaitAck(this SendContext context, bool awaitAck)
        {
            if (!context.TryGetPayload(out RabbitMqSendContext sendContext))
                throw new ArgumentException("The RabbitMqSendContext was not available");

            sendContext.AwaitAck = awaitAck;
        }

        /// <summary>
        /// Sets whether the send should wait for the ack from the broker, or if it should
        /// return immediately after the message is sent by the client.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="awaitAck"></param>
        public static bool TrySetAwaitAck(this SendContext context, bool awaitAck)
        {
            if (!context.TryGetPayload(out RabbitMqSendContext sendContext))
                return false;

            sendContext.AwaitAck = awaitAck;
            return true;
        }
    }
}
