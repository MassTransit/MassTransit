namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Magnum.Extensions;
    using Util;

    public class RabbitMqAddress :
        IEndpointAddress
    {
        static Regex _addressParser = new Regex(@"(?<=/?(?<vhost>\w+)?/(?<type>queue|exchange))(?<path>/[\w/?=]+)",RegexOptions.Compiled);
        const int DEFAULT_PORT = 5432;

		protected static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();
        readonly Uri _rawUri;
        readonly Uri _rebuiltUri;
        readonly bool _isTransactional;
        Func<bool> _isLocal;


        public RabbitMqAddress(Uri uri, Uri rebuiltUri, string username, string password, string vhost, AddressType type, string queue, string args)
        {
            _rawUri = uri;
            _rebuiltUri = rebuiltUri;

            Username = username;
            Password = password;
            Host = rebuiltUri.Host;
            VHost = vhost;
            AddressType = type;
            Queue = queue;
            Port = rebuiltUri.Port;
            _isTransactional = args.GetValueFromQueryString("tx", false);
            _isLocal = ()=>DetermineIfEndpointIsLocal(_rawUri);
        }

        bool DetermineIfEndpointIsLocal(Uri uri)
        {
            string hostName = uri.Host;
			bool local = string.Compare(hostName, ".") == 0 ||
			             string.Compare(hostName, "localhost", true) == 0 ||
			             string.Compare(uri.Host, LocalMachineName, true) == 0;

			Interlocked.Exchange(ref _isLocal, () => local);

			return local;
        }

        public Uri GetConnectionUri()
        {
            return new UriBuilder("", _rawUri.Host, _rawUri.Port, "").Uri;
        }

        public int Port { get; private set; }
        public string Queue { get; private set; }
        public string VHost { get; private set; }
        public string Host { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        
        public Uri RawUri
        {
            get { return _rawUri; }
        }
        public Uri RebuiltUri
        {
            get { return _rebuiltUri; }
        }

        int ParsePort()
        {
            return _rawUri.Port == -1 ? DEFAULT_PORT : _rawUri.Port;
        }

        string ParseVHost(Uri uri)
        {
            string[] bits = uri.LocalPath.Split('/');

            if (bits.Length == 3)
                return @"/";
            else
                return bits[1];
        }
        string ParseQueue(Uri uri)
        {
            string[] bits = uri.LocalPath.Split('/');
            if (bits.Length == 3)
                return bits[2];
            else
                return bits[3];
        }

        #region object overrides
        public bool Equals(RabbitMqAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._rebuiltUri, _rebuiltUri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (RabbitMqAddress)) return false;
            return Equals((RabbitMqAddress) obj);
        }

        public override int GetHashCode()
        {
            return (_rebuiltUri != null ? _rebuiltUri.GetHashCode() : 0);
        }
        #endregion

        public Uri Uri
        {
            get { return _rebuiltUri; }
        }

        public bool IsLocal
        {
            get { return _isLocal(); }
        }

        public string Path
        {
            get { return _rebuiltUri.AbsolutePath.Substring(1); }
        }

        public bool IsTransactional
        {
            get { return _isTransactional; }
        }

        public AddressType AddressType { get; set; }

        public static RabbitMqAddress Parse(string address)
        {
            return Parse(new Uri(address));
        }

        public static RabbitMqAddress Parse(Uri address)
        {
            var port = address.Port == -1 ? DEFAULT_PORT : address.Port;
            var user = "guest";
            var password = "guest";
            if(!address.UserInfo.IsEmpty())
            {
                var parts = address.UserInfo.Split(':');
                user = parts[0];
                password = parts[1];
            }
            var pathParse = _addressParser.Match(address.PathAndQuery);
            var path = pathParse.Groups["path"].Value;
            var vhost = pathParse.Groups["vhost"].Value;
            if (vhost.IsEmpty())
                vhost = "/";

            var type = (AddressType) Enum.Parse(typeof (AddressType), pathParse.Groups["type"].Value, true);
            
            var ub = new UriBuilder(address.Scheme, address.Host, port, address.LocalPath);
            ub.UserName = user;
            ub.Password = password;

            return new RabbitMqAddress(address, ub.Uri, user, password, vhost, type, path, address.Query);
            
        }

        public static RabbitMqAddress ParseForInbound(string address)
        {
            return ParseForInbound(new Uri(address));
        }
        public static RabbitMqAddress ParseForInbound(Uri address)
        {
            return Parse(address);
        }


        public static RabbitMqAddress ParseForOutbound(string address)
        {
            return ParseForOutbound(new Uri(address));
        }

        public static RabbitMqAddress ParseForOutbound(Uri address)
        {
            return Parse(address);
        }
        public static RabbitMqAddress ParseForDuplex(string address)
        {
            return Parse(address);
        }
    }

    public enum AddressType { Exchange, Queue }
}