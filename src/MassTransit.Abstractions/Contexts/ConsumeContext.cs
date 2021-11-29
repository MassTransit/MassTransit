namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public interface ConsumeContext :
        PipeContext,
        MessageContext,
        IPublishEndpoint,
        ISendEndpointProvider
    {
        /// <summary>
        /// The received message context
        /// </summary>
        ReceiveContext ReceiveContext { get; }

        /// <summary>
        /// The serializer context from message deserialization
        /// </summary>
        SerializerContext SerializerContext { get; }

        /// <summary>
        /// An awaitable task that is completed once the consume context is completed
        /// </summary>
        Task ConsumeCompleted { get; }

        /// <summary>
        /// Returns the supported message types from the message
        /// </summary>
        IEnumerable<string> SupportedMessageTypes { get; }

        /// <summary>
        /// Returns true if the specified message type is contained in the serialized message
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        bool HasMessageType(Type messageType);

        /// <summary>
        /// Returns the specified message type if available, otherwise returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consumeContext"></param>
        /// <returns></returns>
        bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class;

        /// <summary>
        /// Add a task that must complete before the consume is completed
        /// </summary>
        /// <param name="task"></param>
        void AddConsumeTask(Task task);

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        Task RespondAsync<T>(T message)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        /// <param name="sendPipe">The pipe used to customize the response send context</param>
        Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        /// <param name="sendPipe">The pipe used to customize the response send context</param>
        Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <param name="message">The message to send</param>
        Task RespondAsync(object message);

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="messageType">The message type to send</param>
        Task RespondAsync(object message, Type messageType);

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="sendPipe"></param>
        Task RespondAsync(object message, IPipe<SendContext> sendPipe);

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="messageType">The message type to send</param>
        /// <param name="sendPipe"></param>
        Task RespondAsync(object message, Type messageType, IPipe<SendContext> sendPipe);

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="values">The values for the message properties</param>
        Task RespondAsync<T>(object values)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="values">The values for the message properties</param>
        /// <param name="sendPipe"></param>
        Task RespondAsync<T>(object values, IPipe<SendContext<T>> sendPipe)
            where T : class;

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acknowledged)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="values">The values for the message properties</param>
        /// <param name="sendPipe"></param>
        Task RespondAsync<T>(object values, IPipe<SendContext> sendPipe)
            where T : class;

        /// <summary>
        /// Adds a response to the message being consumed, which will be sent once the consumer
        /// has completed. The message is not acknowledged until the response is acknowledged.
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        void Respond<T>(T message)
            where T : class;

        /// <summary>
        /// Notify that the message has been consumed -- note that this is internal, and should not be called by a consumer.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType">The consumer type</param>
        Task NotifyConsumed<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class;

        /// <summary>
        /// Notify that a message consumer has faulted -- note that this is internal, and should not be called by a consumer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType">The message consumer type</param>
        /// <param name="exception">The exception that occurred</param>
        Task NotifyFaulted<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class;
    }


    public interface ConsumeContext<out T> :
        ConsumeContext
        where T : class
    {
        T Message { get; }

        /// <summary>
        /// Notify that the message has been consumed -- note that this is internal, and should not be called by a consumer
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="consumerType">The consumer type</param>
        Task NotifyConsumed(TimeSpan duration, string consumerType);

        /// <summary>
        /// Notify that a fault occurred during message consumption -- note that this is internal, and should not be called by a consumer
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception);
    }
}
