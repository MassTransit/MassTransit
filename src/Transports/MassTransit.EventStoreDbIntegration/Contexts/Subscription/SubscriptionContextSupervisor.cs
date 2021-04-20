using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class SubscriptionContextSupervisor :
        TransportPipeContextSupervisor<SubscriptionContext>,
        ISubscriptionContextSupervisor
    {
        public SubscriptionContextSupervisor(IConnectionContextSupervisor supervisor, IHostConfiguration hostConfiguration, SubscriptionSettings subscriptionSettings,
            IHeadersDeserializer headersDeserializer, CheckpointStoreFactory checkpointStoreFactory)
            : base(new SubscriptionContextFactory(supervisor, hostConfiguration, subscriptionSettings, headersDeserializer, checkpointStoreFactory))
        {
            supervisor.AddConsumeAgent(this);
        }
    }
}
