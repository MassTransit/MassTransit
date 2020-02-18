namespace MassTransit.Context.Converters
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Converts the object type message to the appropriate generic type and invokes the send method with that
    /// generic overload.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseEndpointConverter<T> :
        IResponseEndpointConverter
        where T : class
    {
        Task IResponseEndpointConverter.Respond(ConsumeContext consumeContext, object message)
        {
            if (consumeContext == null)
                throw new ArgumentNullException(nameof(consumeContext));
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (message is T msg)
                return consumeContext.RespondAsync(msg);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }

        Task IResponseEndpointConverter.Respond(ConsumeContext consumeContext, object message, IPipe<SendContext> pipe)
        {
            if (consumeContext == null)
                throw new ArgumentNullException(nameof(consumeContext));
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            if (message is T msg)
                return consumeContext.RespondAsync(msg, pipe);

            throw new ArgumentException("Unexpected message type: " + TypeMetadataCache.GetShortName(message.GetType()));
        }
    }
}
