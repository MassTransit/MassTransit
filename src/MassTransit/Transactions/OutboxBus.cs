using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MassTransit.Transactions
{
    public class OutboxBus : BaseOutboxBus, IOutboxBus
    {
        readonly ConcurrentBag<Func<Task>> _pendingActions;

        public OutboxBus(IBus bus)
            : base(bus)
        {
            _pendingActions = new ConcurrentBag<Func<Task>>();
        }

        public override Task Add(Func<Task> action)
        {
            _pendingActions.Add(action);
            return Task.CompletedTask;
        }

        public async Task Release()
        {
            while(_pendingActions.TryTake(out var action))
            {
                await action().ConfigureAwait(false);
            }
        }
    }
}
