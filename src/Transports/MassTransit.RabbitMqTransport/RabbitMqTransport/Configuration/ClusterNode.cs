namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;


    public readonly struct ClusterNode
    {
        public readonly string HostName;
        public readonly int? Port;

        ClusterNode(string hostName, int? port = default)
        {
            HostName = hostName;
            Port = port;
        }

        public override string ToString()
        {
            return Port == -1 ? HostName : $"{HostName}:{Port}";
        }

        public static ClusterNode Parse(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address), "Address must not be null or empty");

            var elements = address.Split(':');

            return elements.Length switch
            {
                1 => new ClusterNode(elements[0]),
                2 when int.TryParse(elements[1], out var port) => new ClusterNode(elements[0], port),
                _ => throw new ArgumentException($"Invalid node address: {address}", nameof(address))
            };
        }
    }
}
