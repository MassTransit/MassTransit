using ProtoBuf;

namespace MassTransit.Serialization
{
#nullable enable
    [ProtoContract]
    [ProtoInclude(100, typeof(HostInfo))]
    public class ProtobufHostInfo : HostInfo
    {
        [ProtoMember(1)]
        public string? MachineName { set; get; }

        [ProtoMember(2)]
        public string? ProcessName { set; get; }

        [ProtoMember(3)]
        public int ProcessId { set; get; }

        [ProtoMember(4)]
        public string? Assembly { set; get; }

        [ProtoMember(5)]
        public string? AssemblyVersion { set; get; }

        [ProtoMember(6)]
        public string? FrameworkVersion { set; get; }

        [ProtoMember(7)]
        public string? MassTransitVersion { set; get; }

        [ProtoMember(8)]
        public string? OperatingSystemVersion { set; get; }
    }
}
