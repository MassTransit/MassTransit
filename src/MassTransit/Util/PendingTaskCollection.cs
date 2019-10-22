namespace MassTransit.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes.Internals.Extensions;


    public class PendingTaskCollection
    {
        readonly IDictionary<long, Task> _tasks;
        long _nextId;

        public PendingTaskCollection(int capacity)
        {
            _tasks = new Dictionary<long, Task>(capacity);
        }

        public void Add(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.Status == TaskStatus.RanToCompletion)
                return;

            var id = Interlocked.Increment(ref _nextId);

            lock (_tasks)
                _tasks.Add(id, task);

            task.ContinueWith(x => Remove(id), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
        }

        public Task Completed(CancellationToken cancellationToken = default) => ReceiveTasksCompleted(cancellationToken);

        async Task ReceiveTasksCompleted(CancellationToken cancellationToken)
        {
            Task[] tasks = null;
            do
            {
                lock (_tasks)
                    tasks = _tasks.Values.Where(x => !x.IsCompletedSuccessfully()).ToArray();

                if (tasks.Length == 0)
                    break;

                var whenAll = Task.WhenAll(tasks);

                if (cancellationToken.CanBeCanceled)
                    whenAll = whenAll.OrCanceled(cancellationToken);

                await whenAll.ConfigureAwait(false);
            }
            while (tasks.Length > 0);
        }

        void Remove(long id)
        {
            lock (_tasks)
                _tasks.Remove(id);
        }
    }
}
