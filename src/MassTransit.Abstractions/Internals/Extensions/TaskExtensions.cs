namespace MassTransit.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;


    public static class TaskExtensions
    {
        static readonly TimeSpan _defaultTimeout = new TimeSpan(0, 0, 0, 5, 0);

        public static Task OrCanceled(this Task task, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
                return task;

            async Task WaitAsync()
            {
                using (RegisterTask(cancellationToken, out var cancelTask))
                {
                    var completed = await Task.WhenAny(task, cancelTask).ConfigureAwait(false);
                    if (completed != task)
                    {
                        task.IgnoreUnobservedExceptions();

                        throw new OperationCanceledException(cancellationToken);
                    }

                    task.GetAwaiter().GetResult();
                }
            }

            return WaitAsync();
        }

        public static Task<T> OrCanceled<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
                return task;

            async Task<T> WaitAsync()
            {
                using (RegisterTask(cancellationToken, out var cancelTask))
                {
                    var completed = await Task.WhenAny(task, cancelTask).ConfigureAwait(false);
                    if (completed != task)
                    {
                        task.IgnoreUnobservedExceptions();

                        throw new OperationCanceledException(cancellationToken);
                    }

                    return task.GetAwaiter().GetResult();
                }
            }

            return WaitAsync();
        }

        public static Task OrTimeout(this Task task, int ms = 0, int s = 0, int m = 0, int h = 0, int d = 0, CancellationToken cancellationToken = default,
            [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null)
        {
            var timeout = new TimeSpan(d, h, m, s, ms);
            if (timeout == TimeSpan.Zero)
                timeout = _defaultTimeout;

            return OrTimeoutInternal(task, timeout, cancellationToken, memberName, filePath, lineNumber);
        }

        public static Task OrTimeout(this Task task, TimeSpan timeout, CancellationToken cancellationToken = default,
            [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null)
        {
            return OrTimeoutInternal(task, timeout, cancellationToken, memberName, filePath, lineNumber);
        }

        static Task OrTimeoutInternal(this Task task, TimeSpan timeout, CancellationToken cancellationToken, string? memberName, string? filePath,
            int? lineNumber)
        {
            if (task.IsCompleted)
                return task;

            async Task WaitAsync()
            {
                var cancel = new CancellationTokenSource();

                var registration = RegisterIfCanBeCanceled(cancellationToken, cancel);
                try
                {
                    var delayTask = Task.Delay(Debugger.IsAttached ? Timeout.InfiniteTimeSpan : timeout, cancel.Token);

                    var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
                    if (completed == delayTask)
                    {
                        task.IgnoreUnobservedExceptions();

                        throw new TimeoutException(FormatTimeoutMessage(memberName, filePath, lineNumber));
                    }

                    task.GetAwaiter().GetResult();
                }
                finally
                {
                    registration.Dispose();
                    cancel.Cancel();
                }
            }

            return WaitAsync();
        }

        public static Task<T> OrTimeout<T>(this Task<T> task, int ms = 0, int s = 0, int m = 0, int h = 0, int d = 0,
            CancellationToken cancellationToken = default,
            [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int? lineNumber = null)
        {
            var timeout = new TimeSpan(d, h, m, s, ms);
            if (timeout == TimeSpan.Zero)
                timeout = _defaultTimeout;

            return OrTimeoutInternal(task, timeout, cancellationToken, memberName, filePath, lineNumber);
        }

        public static Task<T> OrTimeout<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken = default,
            [CallerMemberName] string? memberName = null, [CallerFilePath] string? filePath = null, [CallerLineNumber] int? lineNumber = null)
        {
            return OrTimeoutInternal(task, timeout, cancellationToken, memberName, filePath, lineNumber);
        }

        static Task<T> OrTimeoutInternal<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken, string? memberName, string? filePath,
            int? lineNumber)
        {
            if (task.IsCompleted)
                return task;

            async Task<T> WaitAsync()
            {
                var cancel = new CancellationTokenSource();

                var registration = RegisterIfCanBeCanceled(cancellationToken, cancel);
                try
                {
                    var delayTask = Task.Delay(Debugger.IsAttached ? Timeout.InfiniteTimeSpan : timeout, cancel.Token);

                    var completed = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
                    if (completed == delayTask)
                    {
                        task.IgnoreUnobservedExceptions();

                        throw new TimeoutException(FormatTimeoutMessage(memberName, filePath, lineNumber));
                    }

                    return task.GetAwaiter().GetResult();
                }
                finally
                {
                    registration.Dispose();
                    cancel.Cancel();
                }
            }

            return WaitAsync();
        }

        static string FormatTimeoutMessage(string? memberName, string? filePath, int? lineNumber)
        {
            return !string.IsNullOrEmpty(memberName)
                ? $"Operation in {memberName} timed out at {filePath}:{lineNumber}"
                : "Operation timed out";
        }

        /// <summary>
        /// Returns true if a Task was ran to completion (without being cancelled or faulted)
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static bool IsCompletedSuccessfully(this Task task)
        {
            return task.Status == TaskStatus.RanToCompletion;
        }

        public static void IgnoreUnobservedExceptions(this Task task)
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    var _ = task.Exception;
                }

                return;
            }

            task.ContinueWith(t =>
            {
                var _ = t.Exception;
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Register a callback on the <paramref name="cancellationToken" /> which completes the resulting task.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="cancelTask"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static CancellationTokenRegistration RegisterTask(CancellationToken cancellationToken, out Task cancelTask)
        {
            if (!cancellationToken.CanBeCanceled)
                throw new ArgumentException("The cancellationToken must support cancellation", nameof(cancellationToken));

            var source = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            cancelTask = source.Task;

            return cancellationToken.Register(SetCompleted, source);
        }

        static void SetCompleted(object obj)
        {
            if (obj is TaskCompletionSource<bool> source)
                source.TrySetResult(true);
        }

        static CancellationTokenRegistration RegisterIfCanBeCanceled(CancellationToken cancellationToken, CancellationTokenSource source)
        {
            if (cancellationToken.CanBeCanceled)
                return cancellationToken.Register(Cancel, source);

            return default;
        }

        static void Cancel(object obj)
        {
            if (obj is CancellationTokenSource source)
                source.Cancel();
        }
    }
}
