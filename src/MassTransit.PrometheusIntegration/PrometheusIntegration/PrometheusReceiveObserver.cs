namespace MassTransit.PrometheusIntegration
{
    using System;
    using System.Threading.Tasks;


    public class PrometheusReceiveObserver :
        IReceiveObserver
    {
        public Task PreReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            PrometheusMetrics.MeasureReceived(context);

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            PrometheusMetrics.MeasureConsume(context, duration, consumerType);

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            PrometheusMetrics.MeasureConsume(context, duration, consumerType, exception);

            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            PrometheusMetrics.MeasureReceived(context, exception);

            return Task.CompletedTask;
        }
    }
}
