namespace MassTransit.Middleware
{
    using System;


    public interface ITeeFilter<TContext> :
        IFilter<TContext>,
        IPipeConnector<TContext>
        where TContext : class, PipeContext
    {
        int Count { get; }
    }


    public interface ITeeFilter<TContext, in TKey> :
        ITeeFilter<TContext>,
        IKeyPipeConnector<TKey>
        where TContext : class, PipeContext
    {
    }


    public interface IRequestIdTeeFilter<TMessage> :
        ITeeFilter<ConsumeContext<TMessage>>,
        IKeyPipeConnector<TMessage, Guid>
        where TMessage : class
    {
    }
}
