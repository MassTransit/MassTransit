namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Util;


    public static class BusHandleExtensions
    {
        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop
        /// </summary>
        /// <param name="handle">The bus handle</param>
        public static void Stop(this BusHandle handle)
        {
            TaskUtil.Await(() => handle.StopAsync());
        }

        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop in the specified timeout
        /// </summary>
        /// <param name="handle">The bus handle</param>
        /// <param name="stopTimeout">The wait time before throwing an exception</param>
        public static void Stop(this BusHandle handle, TimeSpan stopTimeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(stopTimeout);

            var cancellationToken = cancellationTokenSource.Token;

            TaskUtil.Await(() => handle.StopAsync(cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Stop a bus, throwing an exception if the bus does not stop in the specified timeout
        /// </summary>
        /// <param name="handle">The bus handle</param>
        /// <param name="stopTimeout">The wait time before throwing an exception</param>
        public static async Task StopAsync(this BusHandle handle, TimeSpan stopTimeout)
        {
            using var cancellationTokenSource = new CancellationTokenSource(stopTimeout);

            await handle.StopAsync(cancellationTokenSource.Token).ConfigureAwait(false);
        }
    }
}
