namespace MassTransit.Testing.Implementations
{
    using System;
    using System.Threading.Tasks;


    public class BusTestReceiveObserver :
        InactivityTestObserver,
        IReceiveObserver
    {
        public BusTestReceiveObserver(TimeSpan inactivityTimout)
        {
            StartTimer(inactivityTimout);
        }

        public Task PreReceive(ReceiveContext context)
        {
            return RestartTimer();
        }

        public Task PostReceive(ReceiveContext context)
        {
            return RestartTimer(false);
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            return RestartTimer(false);
        }
    }
}
