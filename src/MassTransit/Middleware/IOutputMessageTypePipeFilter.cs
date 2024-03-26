namespace MassTransit.Middleware
{
    using System;


    public interface IConsumeContextOutputMessageTypeFilter<out TMessage> :
        IFilter<ConsumeContext>,
        IPipeConnector<ConsumeContext<TMessage>>,
        IConsumeMessageObserverConnector<TMessage>
        where TMessage : class
    {
        ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe);
    }
}
