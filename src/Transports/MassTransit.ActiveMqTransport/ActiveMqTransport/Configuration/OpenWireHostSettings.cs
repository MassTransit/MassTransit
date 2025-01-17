using System;

namespace MassTransit.ActiveMqTransport.Configuration
{
    public class OpenWireHostSettings : ConfigurationHostSettings
    {
        public OpenWireHostSettings(Uri address)
            : base(address)
        {
            TransportOptions["wireFormat.tightEncodingEnabled"] = "true";
        }

        public override string NmsScheme => "activemq";

        public override string HostScheme => UseSsl ? "ssl" : "tcp";

        public override string FailoverScheme => $"{NmsScheme}:failover";

        public override string Scheme => $"{NmsScheme}:{HostScheme}";
    }
}
