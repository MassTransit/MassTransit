namespace MassTransit.Middleware
{
    using System.Net.Mime;
    using System.Threading.Tasks;


    /// <summary>
    /// Sets the CorrelationId header uses the supplied implementation.
    /// </summary>
    /// <typeparam name="T">The message type</typeparam>
    public class SetSerializerFilter<T> :
        IFilter<SendContext<T>>
        where T : class
    {
        readonly ContentType _contentType;

        public SetSerializerFilter(ContentType contentType)
        {
            _contentType = contentType;
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (context.Serialization.TryGetMessageSerializer(_contentType, out var serializer))
                context.Serializer = serializer;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("SetMessageSerializer");
        }
    }
}
