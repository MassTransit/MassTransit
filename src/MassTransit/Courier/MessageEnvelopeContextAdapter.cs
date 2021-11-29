namespace MassTransit.Courier
{
    using System.Threading.Tasks;
    using Serialization;


    public class MessageEnvelopeContextAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly SerializerContext _context;
        readonly MessageEnvelope _envelope;

        public MessageEnvelopeContextAdapter(SerializerContext context, MessageEnvelope envelope)
        {
            _context = context;
            _envelope = envelope;
        }

        public Task Send(SendContext<T> context)
        {
            context.Serializer = _context.GetMessageSerializer(_envelope, context.Message);

            return Task.CompletedTask;
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}
