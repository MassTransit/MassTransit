namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Contexts;
    using GreenPipes.Agents;
    using Transport;


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
            (var subscriptionClient, var sessionClient) = connectionContext.CreateSubscriptionClient(_settings);

            return new SubscriptionClientContext(connectionContext, subscriptionClient, sessionClient, inputAddress, _settings, agent);
        }
    }
}
