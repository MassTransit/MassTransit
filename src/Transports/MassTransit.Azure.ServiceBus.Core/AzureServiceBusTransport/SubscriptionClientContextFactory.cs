namespace MassTransit.AzureServiceBusTransport
{
    using System;


    public class SubscriptionClientContextFactory :
        ClientContextFactory
    {
        readonly SubscriptionSettings _settings;

        public SubscriptionClientContextFactory(IConnectionContextSupervisor supervisor, SubscriptionSettings settings)
            : base(supervisor, settings)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(ConnectionContext connectionContext, Uri inputAddress, IAgent agent)
        {
            return new SubscriptionClientContext(connectionContext, inputAddress, _settings, agent);
        }
    }
}
