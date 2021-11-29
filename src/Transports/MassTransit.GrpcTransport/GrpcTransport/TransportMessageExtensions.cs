namespace MassTransit.GrpcTransport
{
    using System;
    using Contracts;
    using Google.Protobuf.WellKnownTypes;
    using Metadata;


    public static class TransportMessageExtensions
    {
        public static Node Initialize(this Node node, IGrpcHostNode hostNode)
        {
            node.Address = hostNode.NodeAddress.ToString();
            node.Version = "1.0-alpha";
            node.SessionId = hostNode.SessionId.ToString("D");

            node.Host.Add(nameof(HostMetadataCache.Host.Assembly), HostMetadataCache.Host.Assembly);
            node.Host.Add(nameof(HostMetadataCache.Host.AssemblyVersion), HostMetadataCache.Host.AssemblyVersion);
            node.Host.Add(nameof(HostMetadataCache.Host.FrameworkVersion), HostMetadataCache.Host.FrameworkVersion);
            node.Host.Add(nameof(HostMetadataCache.Host.MachineName), HostMetadataCache.Host.MachineName);
            node.Host.Add(nameof(HostMetadataCache.Host.ProcessId), HostMetadataCache.Host.ProcessId.ToString());
            node.Host.Add(nameof(HostMetadataCache.Host.ProcessName), HostMetadataCache.Host.ProcessName);
            node.Host.Add(nameof(HostMetadataCache.Host.MassTransitVersion), HostMetadataCache.Host.MassTransitVersion);
            node.Host.Add(nameof(HostMetadataCache.Host.OperatingSystemVersion), HostMetadataCache.Host.OperatingSystemVersion);

            node.Topology.AddRange(hostNode.GetTopology());

            return node;
        }

        public static DateTime? ToDateTime(this NullableTimestamp value)
        {
            return value is {TimestampCase: NullableTimestamp.TimestampOneofCase.Value}
                ? value.Value.ToDateTime()
                : default;
        }

        public static NullableTimestamp ToFutureDateTime(this TimeSpan? value)
        {
            return value.HasValue
                ? new NullableTimestamp {Value = Timestamp.FromDateTime(DateTime.UtcNow + value.Value)}
                : default;
        }
    }
}
