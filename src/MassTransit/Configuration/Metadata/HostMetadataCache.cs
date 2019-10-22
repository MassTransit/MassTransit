namespace MassTransit.Metadata
{
    public static class HostMetadataCache
    {
        public static HostInfo Host => Cached.HostInfo;


        static class Cached
        {
            internal static readonly HostInfo HostInfo = new BusHostInfo(true);
        }
    }
}
