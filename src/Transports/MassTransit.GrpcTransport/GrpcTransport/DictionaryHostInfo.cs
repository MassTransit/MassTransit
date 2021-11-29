namespace MassTransit.GrpcTransport
{
    using System.Collections.Generic;


    class DictionaryHostInfo :
        HostInfo
    {
        public DictionaryHostInfo(IReadOnlyDictionary<string, string> host)
        {
            if (host.ContainsKey(nameof(MachineName)))
                MachineName = host[nameof(MachineName)];
            if (host.ContainsKey(nameof(ProcessName)))
                ProcessName = host[nameof(ProcessName)];
            if (host.ContainsKey(nameof(ProcessId)) && int.TryParse(host[nameof(ProcessId)], out var processId))
                ProcessId = processId;
            if (host.ContainsKey(nameof(Assembly)))
                Assembly = host[nameof(Assembly)];
            if (host.ContainsKey(nameof(AssemblyVersion)))
                AssemblyVersion = host[nameof(AssemblyVersion)];
            if (host.ContainsKey(nameof(FrameworkVersion)))
                FrameworkVersion = host[nameof(FrameworkVersion)];
            if (host.ContainsKey(nameof(MassTransitVersion)))
                MassTransitVersion = host[nameof(MassTransitVersion)];
            if (host.ContainsKey(nameof(OperatingSystemVersion)))
                OperatingSystemVersion = host[nameof(OperatingSystemVersion)];
        }

        public string MachineName { get; }
        public string ProcessName { get; }
        public int ProcessId { get; }
        public string Assembly { get; }
        public string AssemblyVersion { get; }
        public string FrameworkVersion { get; }
        public string MassTransitVersion { get; }
        public string OperatingSystemVersion { get; }
    }
}
