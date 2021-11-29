namespace MassTransit.Transports
{
    using System.Threading.Tasks;


    public class PublishSendPipeAdapter<T> :
        IPipe<SendContext<T>>
        where T : class
    {
        readonly IPipe<PublishContext<T>> _pipe;

        public PublishSendPipeAdapter(IPipe<PublishContext<T>> pipe)
        {
            _pipe = pipe;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _pipe.Probe(context);
        }

        public Task Send(SendContext<T> context)
        {
            var publishContext = context.GetPayload<PublishContext<T>>();

            return _pipe.Send(publishContext);
        }
    }
}
