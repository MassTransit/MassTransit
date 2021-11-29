namespace MassTransit.PrometheusIntegration
{
    using System;
    using System.Threading.Tasks;


    public class PrometheusSendObserver :
        ISendObserver
    {
        public Task PreSend<T>(SendContext<T> context)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostSend<T>(SendContext<T> context)
            where T : class
        {
            PrometheusMetrics.MeasureSend<T>();

            return Task.CompletedTask;
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            PrometheusMetrics.MeasureSend<T>(exception);

            return Task.CompletedTask;
        }
    }
}
