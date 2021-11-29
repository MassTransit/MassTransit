namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An observer that updates the performance counters using the bus events
    /// generated.
    /// </summary>
    public class PerformanceCounterReceiveObserver :
        IReceiveObserver
    {
        readonly ICounterFactory _factory;

        public PerformanceCounterReceiveObserver(ICounterFactory factory)
        {
            _factory = factory;
        }

        Task IReceiveObserver.PreReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        Task IReceiveObserver.PostReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        Task IReceiveObserver.PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
        {
            ConsumerPerformanceCounterCache.GetCounter(_factory, consumerType).Consumed(duration);
            MessagePerformanceCounterCache<T>.Counter(_factory).Consumed(duration);
            return Task.CompletedTask;
        }

        Task IReceiveObserver.ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
        {
            ConsumerPerformanceCounterCache.GetCounter(_factory, consumerType).Faulted();
            MessagePerformanceCounterCache<T>.Counter(_factory).ConsumeFaulted(duration);
            return Task.CompletedTask;
        }

        Task IReceiveObserver.ReceiveFault(ReceiveContext context, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
