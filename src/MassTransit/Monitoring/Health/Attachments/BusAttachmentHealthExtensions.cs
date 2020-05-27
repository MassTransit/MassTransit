namespace MassTransit.Monitoring.Health
{
    using System;
    using Attachments;


    static class BusAttachmentHealthExtensions
    {
        public static void ConnectHealthCheck(this IBusAttachmentFactoryConfigurator configurator, BusHealth busHealth)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (busHealth == null)
                throw new ArgumentNullException(nameof(busHealth));

            busHealth.Configure(configurator);
        }
    }
}
