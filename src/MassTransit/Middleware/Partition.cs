namespace MassTransit.Middleware
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class Partition :
        IAsyncDisposable
    {
        readonly int _index;
        readonly SemaphoreSlim _limit;
        long _attemptCount;
        long _failureCount;
        long _successCount;

        public Partition(int index)
        {
            _index = index;
            _limit = new SemaphoreSlim(1);
        }

        public async ValueTask DisposeAsync()
        {
            await _limit.WaitAsync().ConfigureAwait(false);

            _limit.Dispose();
        }

        public void Probe(ProbeContext context)
        {
            var partitionScope = context.CreateScope($"partition-{_index}");
            partitionScope.Set(new
            {
                AttemptCount = _attemptCount,
                SuccessCount = _successCount,
                FailureCount = _failureCount
            });
        }

        public async Task Send<T>(T context, IPipe<T> next)
            where T : class, PipeContext
        {
            await _limit.WaitAsync(context.CancellationToken).ConfigureAwait(false);

            try
            {
                Interlocked.Increment(ref _attemptCount);

                await next.Send(context).ConfigureAwait(false);

                Interlocked.Increment(ref _successCount);
            }
            catch
            {
                Interlocked.Increment(ref _failureCount);
                throw;
            }
            finally
            {
                _limit.Release();
            }
        }
    }
}
