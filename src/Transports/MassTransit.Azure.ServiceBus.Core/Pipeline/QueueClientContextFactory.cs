namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using Contexts;
    using GreenPipes.Agents;
    using Transport;


    public class QueueClientContextFactory :
        ClientContextFactory
    {
        readonly ReceiveSettings _settings;

        public QueueClientContextFactory(IConnectionContextSupervisor supervisor, ReceiveSettings settings)
            : base(supervisor, settings)
        {
            _settings = settings;
        }

        protected override ClientContext CreateClientContext(ConnectionContext connectionContext, Uri inputAddress, IAgent agent)
        {
            return new QueueClientContext(connectionContext, inputAddress, _settings, agent);
        }
    }
}
