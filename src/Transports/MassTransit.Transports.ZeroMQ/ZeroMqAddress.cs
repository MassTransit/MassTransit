namespace MassTransit.Transports.ZeroMq
{
    using System;
    using System.Diagnostics;
    using ZMQ;

    [DebuggerDisplay("ZMQ: {RebuiltUri}")]
    public class ZeroMqAddress : IZeroMqEndpointAddress
    {
    	readonly Uri _rawUri;
    	readonly Uri _rebuiltUri;
        bool _isTransactional;
        bool _isLocal;

        public ZeroMqAddress(Uri uri)
        {
            _rawUri = uri;

            _isLocal = true;
            _isTransactional = false;

            Port = uri.Port;
            Host = uri.Host;
            ZmqTransport = getTransport(uri); //needs to change based on incoming scheme
            

            var builder = new UriBuilder(ZmqTransport.ToString(), Host,Port);

            _rebuiltUri = builder.Uri;
        }

        Transport getTransport(Uri uri)
        {
            var substring = uri.Scheme.Replace("zeromq-","");
            return (Transport)Enum.Parse(typeof(Transport), substring, true);
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public Transport ZmqTransport { get; private set; }

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

        public Uri Uri
        {
            get { return RebuiltUri; }
        }

        public bool IsLocal
        {
            get { return _isLocal; }
        }

        public bool IsTransactional
        {
            get { return _isTransactional; }
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

        public static ZeroMqAddress Parse(Uri uri)
        {
            return new ZeroMqAddress(uri);
        }
    }
}
