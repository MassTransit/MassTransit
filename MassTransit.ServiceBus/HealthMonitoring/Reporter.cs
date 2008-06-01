namespace MassTransit.ServiceBus.HealthMonitoring
{
    using log4net;
    using Messages;

    public class Reporter : 
        Consumes<DownEndpoint>.All
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (Reporter));

        public void Consume(DownEndpoint message)
        {
            _log.ErrorFormat("Endpoint '{0}' is down", message.Endpoint);
        }
    }
}