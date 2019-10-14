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
        readonly CancellationToken _cancellationToken;
        readonly IDictionary<long, Task> _tasks;
        long _nextId;

        public PendingTaskCollection(CancellationToken cancellationToken = default)
            : this(4, cancellationToken)
        {
        }

        public PendingTaskCollection(int capacity, CancellationToken cancellationToken = default)
        {
            _cancellationToken = cancellationToken;
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

        public Task Completed => ReceiveTasksCompleted();

        async Task ReceiveTasksCompleted()
        {
            Task[] tasks = null;
            do
            {
                lock (_tasks)
                    tasks = _tasks.Values.Where(x => !x.IsCompletedSuccessfully()).ToArray();

                if (tasks.Length == 0)
                    break;

                var whenAll = Task.WhenAll(tasks);

                if (_cancellationToken.CanBeCanceled)
                    whenAll = whenAll.OrCanceled(_cancellationToken);

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
