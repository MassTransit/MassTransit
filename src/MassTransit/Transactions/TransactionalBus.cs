namespace MassTransit.Transactions
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;


    public class TransactionalBus :
        BaseTransactionalBus,
        ITransactionalBus
    {
        readonly ConcurrentBag<Func<Task>> _pendingActions;

        public TransactionalBus(IBus bus)
            : base(bus)
        {
            _pendingActions = new ConcurrentBag<Func<Task>>();
        }

        public async Task Release()
        {
            while (_pendingActions.TryTake(out Func<Task> action))
                await action().ConfigureAwait(false);
        }

        public override Task Add(Func<Task> action)
        {
            _pendingActions.Add(action);
            return Task.CompletedTask;
        }
    }
}
