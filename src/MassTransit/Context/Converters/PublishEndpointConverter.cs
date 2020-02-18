namespace MassTransit.Context.Converters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Converts the object message type to the generic type T and publishes it on the endpoint specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PublishEndpointConverter<T> :
        IPublishEndpointConverter
        where T : class
    {
        Task IPublishEndpointConverter.Publish(IPublishEndpoint endpoint, object message, CancellationToken cancellationToken)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is T msg)
                return endpoint.Publish(msg, cancellationToken);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }

        Task IPublishEndpointConverter.Publish(IPublishEndpoint endpoint, object message, IPipe<PublishContext> pipe, CancellationToken cancellationToken)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            if (message is T msg)
                return endpoint.Publish(msg, pipe, cancellationToken);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }
    }
}
