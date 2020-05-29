namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Threading.Tasks;
    using Util;


    /// <summary>
    /// An observer that updates the performance counters using the bus events
    /// generated.
    /// </summary>
    public class PerformanceCounterPublishObserver :
        IPublishObserver
    {
        readonly ICounterFactory _factory;

        public PerformanceCounterPublishObserver(ICounterFactory factory)
        {
            _factory = factory;
        }

        Task IPublishObserver.PrePublish<T>(PublishContext<T> context)
        {
            return TaskUtil.Completed;
        }

        Task IPublishObserver.PostPublish<T>(PublishContext<T> context)
        {
            MessagePerformanceCounterCache<T>.Counter(_factory).Published();

            return TaskUtil.Completed;
        }

        Task IPublishObserver.PublishFault<T>(PublishContext<T> context, Exception exception)
        {
            MessagePerformanceCounterCache<T>.Counter(_factory).PublishFaulted();

            return TaskUtil.Completed;
        }
    }
}
