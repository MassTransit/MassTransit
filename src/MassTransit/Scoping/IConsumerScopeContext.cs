namespace MassTransit.Scoping
{
    using System;


    public interface IConsumerScopeContext :
        IDisposable
    {
        ConsumeContext Context { get; }
    }


    public interface IConsumerScopeContext<out TConsumer, out T> :
        IDisposable
        where T : class
        where TConsumer : class
    {
        ConsumerConsumeContext<TConsumer, T> Context { get; }
    }
}
