namespace MassTransit.Transports.ZeroMQ
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("ZMQ: {RebuiltUri}")]
    public class ZeroMqAddress
    {
    	readonly Uri _rawUri;
    	readonly Uri _rebuiltUri;

        public ZeroMqAddress(Uri uri)
        {
            _rawUri = uri;

            Port = uri.Port;
            Host = uri.Host;

            var builder = new UriBuilder("tcp",Host,Port);

            _rebuiltUri = builder.Uri;
        }

        public string Host { get; set; }
        public int Port { get; set; }

        public Uri RawUri
        {
            get { return _rawUri; }
        }
        public Uri RebuiltUri
        {
            get { return _rebuiltUri; }
        }

        public Uri GetConnectionUri()
        {
            return RebuiltUri;
        }

        #region object overrides
        public bool Equals(ZeroMqAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._rebuiltUri, _rebuiltUri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ZeroMqAddress)) return false;
            return Equals((ZeroMqAddress) obj);
        }

        public override int GetHashCode()
        {
            return (_rebuiltUri != null ? _rebuiltUri.GetHashCode() : 0);
        }
        #endregion
    }
}
