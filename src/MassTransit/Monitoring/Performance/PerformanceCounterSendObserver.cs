namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An observer that updates the performance counters using the bus events
    /// generated.
    /// </summary>
    public class PerformanceCounterSendObserver :
        ISendObserver
    {
        readonly ICounterFactory _factory;

        public PerformanceCounterSendObserver(ICounterFactory factory)
        {
            _factory = factory;
        }

        Task ISendObserver.PreSend<T>(SendContext<T> context)
        {
            return Task.CompletedTask;
        }

        Task ISendObserver.PostSend<T>(SendContext<T> context)
        {
            MessagePerformanceCounterCache<T>.Counter(_factory).Sent();

            return Task.CompletedTask;
        }

        Task ISendObserver.SendFault<T>(SendContext<T> context, Exception exception)
        {
            MessagePerformanceCounterCache<T>.Counter(_factory).SendFaulted();

            return Task.CompletedTask;
        }
    }
}
