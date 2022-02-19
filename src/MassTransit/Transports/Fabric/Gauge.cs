namespace MassTransit.Transports.Fabric
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class Gauge :
        Metric
    {
        long _activeCount;
        long _concurrentActiveCount;

        public event ZeroActiveHandler ZeroActive;

        public void Add()
        {
            var currentActiveCount = Interlocked.Increment(ref _activeCount);
            while (currentActiveCount < _concurrentActiveCount)
                Interlocked.CompareExchange(ref _concurrentActiveCount, currentActiveCount, _concurrentActiveCount);
        }

        public void Remove()
        {
            var pendingCount = Interlocked.Decrement(ref _activeCount);
            if (pendingCount != 0)
                return;

            var zeroActivity = ZeroActive;
            if (zeroActivity == null)
                return;

            Task.Run(() => NotifyZeroActivity(zeroActivity));
        }

        static Task NotifyZeroActivity(ZeroActiveHandler zeroActivity)
        {
            Delegate[] invocationList = zeroActivity.GetInvocationList();

            async Task InvokeAsync()
            {
                for (var i = 0; i < invocationList.Length; i++)
                {
                    if (invocationList[i] is ZeroActiveHandler handler)
                        await handler().ConfigureAwait(false);
                }
            }

            return invocationList.Length switch
            {
                0 => default,
                1 when invocationList[0] is ZeroActiveHandler handler => handler(),
                _ => InvokeAsync()
            };
        }
    }
}
