using System;
using MassTransit.Transports.Msmq;

namespace BusDriver.Commands
{
    public static class From
    {
        public static CreateMsmqTransportSettings Uri(Uri uri)
        {
            return new CreateMsmqTransportSettings(uri)
            {
                Transactional = false
            };
        }

        public static CreateMsmqTransportSettings Transactional(
            this CreateMsmqTransportSettings createMsmqTransportSettings)
        {
            createMsmqTransportSettings.Transactional = true;
            return createMsmqTransportSettings;
        }
    }
}
