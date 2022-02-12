namespace MassTransit.Metadata
{
    using System;


    public static class HostMetadataCache
    {
        static bool? _isRunningInContainer;

        public static HostInfo Host => Cached.HostInfo;
        public static HostInfo Empty => Cached.EmptyHostInfo;

        public static bool IsRunningInContainer =>
            _isRunningInContainer ??= bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inDocker) && inDocker;


        static class Cached
        {
            internal static readonly HostInfo HostInfo = new BusHostInfo(true);
            internal static readonly HostInfo EmptyHostInfo = new BusHostInfo();
        }
    }
}
