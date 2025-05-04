namespace MassTransit.InMemoryTransport;

using System;
using System.Threading;
using System.Threading.Tasks;


public interface IInMemoryDelayProvider
{
    Task Delay(int milliseconds, CancellationToken cancellationToken = default);
    Task Delay(TimeSpan delay, CancellationToken cancellationToken = default);
    Task Delay(DateTime delayUntil, CancellationToken cancellationToken = default);

    ValueTask Advance(TimeSpan duration);
}
