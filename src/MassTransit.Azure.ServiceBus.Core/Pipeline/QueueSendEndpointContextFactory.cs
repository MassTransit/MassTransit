namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Context;
    using Contexts;
    using GreenPipes;
    using Transport;


    public class QueueSendEndpointContextFactory :
        SendEndpointContextFactory
    {
        readonly SendSettings _settings;

        public QueueSendEndpointContextFactory(IConnectionContextSupervisor supervisor, IPipe<SendEndpointContext> pipe, SendSettings settings)
            : base(supervisor, pipe)
        {
            _settings = settings;
        }

        protected override SendEndpointContext CreateSendEndpointContext(ConnectionContext context)
        {
            LogContext.Debug?.Log("Creating Queue Client: {Queue}", _settings.EntityPath);

            var messageSender = context.CreateMessageSender(_settings.EntityPath);

            return new QueueSendEndpointContext(context, messageSender);
        }
    }
}
