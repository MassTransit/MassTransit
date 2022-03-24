namespace MassTransit.Monitoring
{
    using System;
    using System.Threading.Tasks;


    public class InstrumentSendObserver :
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
            Instrumentation.MeasureSend<T>();

            return Task.CompletedTask;
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception)
            where T : class
        {
            Instrumentation.MeasureSend<T>(exception);

            return Task.CompletedTask;
        }
    }
}
