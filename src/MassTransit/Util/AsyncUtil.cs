namespace MassTransit.Util
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class AsyncUtil
    {
        private static readonly TaskFactory taskFactory =
            new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static void RunSync(Func<Task> func)
        {
            taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

        public static T RunSync<T>(Func<Task<T>> func)
        {
            return taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
        }

    }
}
