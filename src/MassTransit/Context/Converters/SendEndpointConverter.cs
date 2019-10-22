namespace MassTransit.Context.Converters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Converts the object type message to the appropriate generic type and invokes the send method with that
    /// generic overload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SendEndpointConverter<T> :
        ISendEndpointConverter
        where T : class
    {
        Task ISendEndpointConverter.Send(ISendEndpoint endpoint, object message, CancellationToken cancellationToken)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is T msg)
                return endpoint.Send(msg, cancellationToken);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }

        Task ISendEndpointConverter.Send(ISendEndpoint endpoint, object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            if (message is T msg)
                return endpoint.Send(msg, pipe, cancellationToken);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }
    }
}
