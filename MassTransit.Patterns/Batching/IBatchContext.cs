using System.Collections.Generic;
using MassTransit.ServiceBus;

namespace MassTransit.Patterns.Batching
{
    public interface IBatchContext<T, K> :
        IEnumerable<T>
    {
        K BatchId { get; }

        IEndpoint ReturnEndpoint { get; }

        IServiceBus Bus { get; }

        bool IsComplete { get; }

        void Enqueue(T message);
    }
}