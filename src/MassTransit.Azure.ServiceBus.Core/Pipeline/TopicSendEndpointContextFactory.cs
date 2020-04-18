namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using Context;
    using Contexts;
    using GreenPipes;
    using Transport;


    public class TopicSendEndpointContextFactory :
        SendEndpointContextFactory
    {
        readonly SendSettings _settings;

        public TopicSendEndpointContextFactory(IConnectionContextSupervisor supervisor, IPipe<SendEndpointContext> pipe, SendSettings settings)
            : base(supervisor, pipe)
        {
            _settings = settings;
        }

        protected override SendEndpointContext CreateSendEndpointContext(ConnectionContext context)
        {
            LogContext.Debug?.Log("Creating Topic Client: {Topic}", _settings.EntityPath);

            var messageSender = context.CreateMessageSender(_settings.EntityPath);

            return new TopicSendEndpointContext(context, messageSender);
        }
    }
}
