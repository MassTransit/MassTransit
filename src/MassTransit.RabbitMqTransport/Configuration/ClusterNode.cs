namespace MassTransit.RabbitMqTransport
{
    using System;


    public readonly struct ClusterNode
    {
        public readonly string HostName;
        public readonly int Port;

        public ClusterNode(string hostName, int port = -1)
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

            string[] elements = address.Split(':');
            switch (elements.Length)
            {
                case 1:
                    return new ClusterNode(elements[0]);
                    break;
                case 2 when int.TryParse(elements[1], out var port):
                    return new ClusterNode(elements[0], port);
                    break;
                default:
                    throw new ArgumentException($"Invalid node address: {address}", nameof(address));
            }
        }
    }
}
