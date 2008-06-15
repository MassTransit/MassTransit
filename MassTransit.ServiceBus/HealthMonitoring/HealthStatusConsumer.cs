namespace MassTransit.ServiceBus.HealthMonitoring
{
    using System.Collections.Generic;
    using Messages;

    public class HealthStatusConsumer :
        Consumes<HealthStatusRequest>.All
    {
        private readonly IServiceBus _bus;
        private readonly IHealthCache _cache;

        public HealthStatusConsumer(IServiceBus bus, IHealthCache cache)
        {
            _bus = bus;
            _cache = cache;
        }

        #region Implementation of All

        public void Consume(HealthStatusRequest message)
        {
            IList<HealthInformation> _updates = _cache.List();
            _bus.Publish(new HealthStatusResponse(_updates, message.CorrelationId));
        }

        #endregion
    }
}