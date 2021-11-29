namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A publish endpoint lets the underlying transport determine the actual endpoint to which
    /// the message is sent. For example, an exchange on RabbitMQ and a topic on Azure Service bus.
    /// </summary>
    public interface IPublishEndpoint :
        IPublishObserverConnector
    {
        /// <summary>
        /// <para>
        /// Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.
        /// </para>
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// <para>
        /// Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.
        /// </para>
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// <para>
        /// Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.
        /// </para>
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        Task Publish(object message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        Task Publish(object message, Type messageType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default);

        /// <summary>
        /// <see cref="IPublishEndpoint.Publish{T}(T,CancellationToken)" />: this is a "dynamically"
        /// typed overload - give it an interface as its type parameter,
        /// and a loosely typed dictionary of values and the MassTransit
        /// underlying infrastructure will populate an object instance
        /// with the passed values. It actually does this with DynamicProxy
        /// in the background.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the interface or
        /// non-sealed class with all-virtual members.
        /// </typeparam>
        /// <param name="values">
        /// The dictionary of values to place in the
        /// object instance to implement the interface.
        /// </param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// <see cref="IPublishEndpoint.Publish{T}(T,CancellationToken)" />: this
        /// overload further takes an action; it allows you to set <see cref="PublishContext" />
        /// meta-data. Also <see cref="IPublishEndpoint.Publish{T}(T,CancellationToken)" />.
        /// </summary>
        /// <typeparam name="T">The type of the message to publish</typeparam>
        /// <param name="values">
        /// The dictionary of values to become hydrated and
        /// published under the type of the interface.
        /// </param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class;

        /// <summary>
        /// <see cref="IPublishEndpoint.Publish{T}(T,CancellationToken)" />: this
        /// overload further takes an action; it allows you to set <see cref="PublishContext" />
        /// meta-data. Also <see cref="IPublishEndpoint.Publish{T}(T,CancellationToken)" />.
        /// </summary>
        /// <typeparam name="T">The type of the message to publish</typeparam>
        /// <param name="values">
        /// The dictionary of values to become hydrated and
        /// published under the type of the interface.
        /// </param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class;
    }
}
