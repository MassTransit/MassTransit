namespace MassTransit.DependencyInjection
{
    using System;


    public interface IConsumerConsumeScopeContext<out TConsumer, out T> :
        IAsyncDisposable
        where T : class
        where TConsumer : class
    {
        ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}
