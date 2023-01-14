namespace MassTransit.KafkaIntegration.Caching
{
    using System;
    using MassTransit.Caching;


    public interface ICachedTopicProducer<out T> :
        INotifyValueUsed,
        IAsyncDisposable
    {
        T Key { get; }
    }
}
