namespace MassTransit.Configuration
{
    using System;


    /// <summary>
    /// Connects a message handler to a pipe
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class HandlerConnector<TMessage> :
        IHandlerConnector<TMessage>
        where TMessage : class
    {
        public ConnectHandle ConnectHandler(IConsumePipeConnector consumePipe, MessageHandler<TMessage> handler,
            IBuildPipeConfigurator<ConsumeContext<TMessage>> configurator)
        {
            configurator ??= new PipeConfigurator<ConsumeContext<TMessage>>();
            configurator.AddPipeSpecification(new HandlerPipeSpecification<TMessage>(handler));

            return consumePipe.ConnectConsumePipe(configurator.Build());
        }

        public ConnectHandle ConnectRequestHandler(IRequestPipeConnector consumePipe, Guid requestId, MessageHandler<TMessage> handler,
            IBuildPipeConfigurator<ConsumeContext<TMessage>> configurator)
        {
            configurator.AddPipeSpecification(new HandlerPipeSpecification<TMessage>(handler));

            return consumePipe.ConnectRequestPipe(requestId, configurator.Build());
        }
    }
}
