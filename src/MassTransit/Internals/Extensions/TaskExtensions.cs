namespace MassTransit.Internals.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = TaskUtil.GetTask<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            return await task.ConfigureAwait(false);
        }

        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = TaskUtil.GetTask<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                    throw new OperationCanceledException(cancellationToken);

            await task.ConfigureAwait(false);
        }

        public static async Task WithTimeout(this Task task, int milliseconds)
        {
            using (var tokenSource = new CancellationTokenSource(milliseconds))
            {
                var tcs = TaskUtil.GetTask<bool>();
                using (tokenSource.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                    if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                        throw new OperationCanceledException(tokenSource.Token);

                await task.ConfigureAwait(false);
            }
        }

        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            using (var tokenSource = new CancellationTokenSource(timeout))
            {
                var tcs = TaskUtil.GetTask<bool>();
                using (tokenSource.Token.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                    if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(false))
                        throw new OperationCanceledException(tokenSource.Token);

                await task.ConfigureAwait(false);
            }
        }
    }
}
