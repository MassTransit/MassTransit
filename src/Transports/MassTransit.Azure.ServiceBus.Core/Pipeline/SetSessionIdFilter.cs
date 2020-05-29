namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System.Threading.Tasks;
    using GreenPipes;
    using Topology.Conventions;


    public class SetSessionIdFilter<T> :
        IFilter<ServiceBusSendContext<T>>
        where T : class
    {
        readonly IMessageSessionIdFormatter<T> _sessionIdFormatter;

        public SetSessionIdFilter(IMessageSessionIdFormatter<T> sessionIdFormatter)
        {
            _sessionIdFormatter = sessionIdFormatter;
        }

        public Task Send(ServiceBusSendContext<T> context, IPipe<ServiceBusSendContext<T>> next)
        {
            var sessionId = _sessionIdFormatter.FormatSessionId(context);

            if (!string.IsNullOrWhiteSpace(sessionId))
                context.SessionId = sessionId;

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("setSessionId");
        }
    }
}
