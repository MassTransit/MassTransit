namespace MassTransit.SqlTransport
{
    using System;
    using Metadata;
    using Serialization;


    public static class HostInfoCache
    {
        static readonly Lazy<string> _hostInfoJson =
            new Lazy<string>(() => SystemTextJsonMessageSerializer.Instance.SerializeObject(HostMetadataCache.Host).GetString());

        public static string HostInfoJson => _hostInfoJson.Value;
    }
}
