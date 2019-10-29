namespace MassTransit.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;


    public static class TaskUtil
    {
        internal static Task Canceled => Cached<bool>.CanceledTask;
        public static Task Completed => Cached.CompletedTask;
        public static Task<bool> False => Cached.FalseTask;
        public static Task<bool> True => Cached.TrueTask;

        /// <summary>
        /// Returns a completed task with the default value for <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> Default<T>() => Cached<T>.DefaultValueTask;

        /// <summary>
        /// Returns a faulted task with the specified exception (creating using a <see cref="TaskCompletionSource{T}"/>)
        /// </summary>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> Faulted<T>(Exception exception)
        {
            var source = GetTask<T>();
            source.TrySetException(exception);

            return source.Task;
        }

        /// <summary>
        /// Returns a cancelled task for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task<T> Cancelled<T>()
        {
            return Cached<T>.CanceledTask;
        }

        /// <summary>
        /// Creates a new <see cref="TaskCompletionSource{T}"/>, and ensures the <see cref="TaskCreationOptions.RunContinuationsAsynchronously"/>
        /// flag is specified (if available).
        /// </summary>
        /// <param name="options"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TaskCompletionSource<T> GetTask<T>(TaskCreationOptions options = TaskCreationOptions.None)
        {
        #if NETCORE
            options |= TaskCreationOptions.RunContinuationsAsynchronously;
        #endif
            return new TaskCompletionSource<T>(options);
        }

        public static void Await(Func<Task> taskFactory, CancellationToken cancellationToken = default)
        {
            if (taskFactory == null)
                throw new ArgumentNullException(nameof(taskFactory));

            var previousContext = SynchronizationContext.Current;
            try
            {
                var syncContext = new SingleThreadSynchronizationContext(cancellationToken);
                SynchronizationContext.SetSynchronizationContext(syncContext);

                var t = taskFactory();
                if (t == null)
                    throw new InvalidOperationException("The taskFactory must return a Task");

                var awaiter = t.GetAwaiter();

                while (!awaiter.IsCompleted)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException("The task was not completed before being cancelled");

                    syncContext.RunOnCurrentThread(cancellationToken);
                }

                syncContext.SetComplete();

                awaiter.GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        public static void Await(Task task, CancellationToken cancellationToken = default)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var previousContext = SynchronizationContext.Current;
            try
            {
                var syncContext = new SingleThreadSynchronizationContext(cancellationToken);
                SynchronizationContext.SetSynchronizationContext(syncContext);

                var awaiter = task.GetAwaiter();

                while (!awaiter.IsCompleted)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException("The task was not completed before being cancelled");

                    syncContext.RunOnCurrentThread(cancellationToken);
                }

                syncContext.SetComplete();

                awaiter.GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        public static T Await<T>(Func<Task<T>> taskFactory, CancellationToken cancellationToken = default)
        {
            if (taskFactory == null)
                throw new ArgumentNullException(nameof(taskFactory));

            var previousContext = SynchronizationContext.Current;
            try
            {
                var syncContext = new SingleThreadSynchronizationContext(cancellationToken);
                SynchronizationContext.SetSynchronizationContext(syncContext);

                Task<T> t = taskFactory();
                if (t == null)
                    throw new InvalidOperationException("The taskFactory must return a Task");

                TaskAwaiter<T> awaiter = t.GetAwaiter();

                while (!awaiter.IsCompleted)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException("The task was not completed before being cancelled");

                    syncContext.RunOnCurrentThread(cancellationToken);
                }

                syncContext.SetComplete();

                return awaiter.GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }


        static class Cached
        {
            public static readonly Task CompletedTask = Task.FromResult(true);
            public static readonly Task<bool> TrueTask = Task.FromResult(true);
            public static readonly Task<bool> FalseTask = Task.FromResult(false);
        }


        static class Cached<T>
        {
            public static readonly Task<T> DefaultValueTask = Task.FromResult<T>(default);
            public static readonly Task<T> CanceledTask = GetCanceledTask();

            static Task<T> GetCanceledTask()
            {
                var source = GetTask<T>();
                source.SetCanceled();
                return source.Task;
            }
        }


        sealed class SingleThreadSynchronizationContext :
            SynchronizationContext
        {
            readonly CancellationToken _cancellationToken;

            readonly BlockingCollection<Tuple<SendOrPostCallback, object>> _queue;
            bool _completed;

            public SingleThreadSynchronizationContext(CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
                _queue = new BlockingCollection<Tuple<SendOrPostCallback, object>>();
            }

            public override void Post(SendOrPostCallback callback, object state)
            {
                if (callback == null)
                    throw new ArgumentNullException(nameof(callback));

                if (_completed)
                    throw new TaskSchedulerException("The synchronization context was already completed");

                try
                {
                    _queue.Add(Tuple.Create(callback, state), _cancellationToken);
                }
                catch (InvalidOperationException)
                {
                }
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            public void RunOnCurrentThread(CancellationToken cancellationToken)
            {
                Tuple<SendOrPostCallback, object> callback;
                while (_queue.TryTake(out callback, 50, cancellationToken))
                    callback.Item1(callback.Item2);
            }

            public void SetComplete()
            {
                _completed = true;
            }
        }
    }
}
