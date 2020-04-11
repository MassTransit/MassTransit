namespace MassTransit.PrometheusIntegration.Observers
{
    using System;
    using System.Threading.Tasks;


    public class PrometheusPublishObserver :
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
            PrometheusMetrics.MeasurePublished<T>();

            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            PrometheusMetrics.MeasurePublished<T>(exception);

            return Task.CompletedTask;
        }
    }
}
