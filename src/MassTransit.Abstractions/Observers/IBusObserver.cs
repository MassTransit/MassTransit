namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// Used to observe events produced by the bus
    /// </summary>
    public interface IBusObserver
    {
        /// <summary>
        /// Called after the bus has been created.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        void PostCreate(IBus bus);

        /// <summary>
        /// Called when the bus fails to be created
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        void CreateFaulted(Exception exception);

        /// <summary>
        /// Called when the bus is being started, before the actual Start commences.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PreStart(IBus bus);

        /// <summary>
        /// Called once the bus has started and is running
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="busReady">
        /// A task which is completed once the bus is ready and all receive endpoints are ready.
        /// </param>
        /// <returns></returns>
        Task PostStart(IBus bus, Task<BusReady> busReady);

        /// <summary>
        /// Called when the bus fails to start
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task StartFaulted(IBus bus, Exception exception);

        /// <summary>
        /// Called when the bus is being stopped, before the actual Stop commences.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PreStop(IBus bus);

        /// <summary>
        /// Called when the bus has been stopped.
        /// </summary>
        /// <param name="bus"></param>
        /// <returns></returns>
        Task PostStop(IBus bus);

        /// <summary>
        /// Called when the bus failed to Stop.
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        Task StopFaulted(IBus bus, Exception exception);
    }
}
