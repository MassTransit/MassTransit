namespace MassTransit.Monitoring
{
    using System;
    using System.Threading.Tasks;


    public class InstrumentPublishObserver :
        IPublishObserver
    {
        public Task PrePublish<T>(PublishContext<T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context)
            where T : class
        {
            Instrumentation.MeasurePublish<T>();

            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            Instrumentation.MeasurePublish<T>(exception);

            return Task.CompletedTask;
        }
    }
}
