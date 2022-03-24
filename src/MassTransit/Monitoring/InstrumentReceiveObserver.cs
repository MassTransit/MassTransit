namespace MassTransit.Monitoring
{
    using System;
    using System.Threading.Tasks;


    public class InstrumentReceiveObserver :
        IReceiveObserver
    {
        public Task PreReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            Instrumentation.MeasureReceived(context);

            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            Instrumentation.MeasureConsume(context, duration, consumerType);

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            Instrumentation.MeasureConsume(context, duration, consumerType, exception);

            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            Instrumentation.MeasureReceived(context, exception);

            return Task.CompletedTask;
        }
    }
}
