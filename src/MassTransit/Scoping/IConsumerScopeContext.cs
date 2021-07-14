namespace MassTransit.Scoping
{
    using System;


    public interface IConsumerScopeContext :
        IAsyncDisposable
    {
        ConsumeContext Context { get; }
    }


    public interface IConsumerScopeContext<out TConsumer, out T> :
        IAsyncDisposable
        where T : class
        where TConsumer : class
    {
        ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}
