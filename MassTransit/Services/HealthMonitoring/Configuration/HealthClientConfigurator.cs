namespace MassTransit.Services.HealthMonitoring.Configuration
{
    using System;
    using Internal;
    using MassTransit.Configuration;

    public class HealthClientConfigurator :
        IServiceConfigurator
    {
        private int _heartbeatInterval;

        public Type ServiceType
        {
            get { return typeof (HealthClient); }
        }

        public IBusService Create(IServiceBus bus, IObjectBuilder builder)
        {
            var service = new HealthClient(_heartbeatInterval);

            return service;
        }

        public void SetHeartbeatInterval(int interval)
        {
            _heartbeatInterval = interval;
        }
    }
}