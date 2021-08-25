using GreenPipes;
using System.Threading.Tasks;

namespace MassTransit.Transports.Outbox
{
    public class OnRampScopedRepositoryFilter<T> :
        IFilter<PublishContext<T>>,
        IFilter<SendContext<T>>
    where T : class
    {
        private readonly IOnRampTransportRepository _repository;

        public OnRampScopedRepositoryFilter(IOnRampTransportRepository repository)
        {
            _repository = repository;
        }

        public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            context.GetOrAddPayload(() => _repository);
            await next.Send(context);
        }

        public void Probe(ProbeContext context) { }

        public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            context.GetOrAddPayload(() => _repository);
            await next.Send(context);
        }
    }
}
