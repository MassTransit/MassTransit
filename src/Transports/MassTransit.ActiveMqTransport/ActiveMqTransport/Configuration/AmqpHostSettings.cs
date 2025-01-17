using System;

namespace MassTransit.ActiveMqTransport.Configuration
{
    public class AmqpHostSettings : ConfigurationHostSettings
    {
        public AmqpHostSettings(Uri address)
            : base(address)
        {
        }

        public override string HostScheme => UseSsl ? $"{NmsScheme}s" : NmsScheme;

        public override string FailoverScheme => "failover";

        public override string Scheme => HostScheme;

        public override string NmsScheme => "amqp";
    }
}
