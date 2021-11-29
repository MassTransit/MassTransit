namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Returned once a bus has been started. Should call Stop or Dispose before the process
    /// can exit.
    /// </summary>
    public interface BusHandle
    {
        /// <summary>
        /// A task which can be awaited to know when the bus is ready and all of the receive endpoints have reported ready.
        /// </summary>
        Task<BusReady> Ready { get; }

        /// <summary>
        /// Stop the bus and all receiving endpoints on the bus. Note that cancelling the Stop
        /// operation may leave the bus and/or one or more receive endpoints in an indeterminate
        /// state.
        /// </summary>
        /// <param name="cancellationToken">Cancel the stop operation in progress</param>
        /// <returns>An awaitable task that is completed once everything is stopped</returns>
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
