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
        static Regex _addressParser = new Regex(@"(?<scheme>\w+)://(?<host>[\w-]+)(?<port>:\d+)?(?<type>/\w+)(?<path>/[\w/]+)",RegexOptions.Compiled);
        const int DEFAULT_PORT = 5432;

		protected static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();
        readonly Uri _rawUri;
        readonly Uri _rebuiltUri;
        readonly bool _isTransactional;
        Func<bool> _isLocal;


        public RabbitMqAddress(Uri uri, Uri rebuiltUri, string username, string password, string vhost)
        {
            _rawUri = uri;
            _rebuiltUri = rebuiltUri;

            Username = username;
            Password = password;
            Host = rebuiltUri.Host;
            VHost = vhost;
            AddressType = ParseExchange(_rawUri);
            Queue = ParseQueue(_rawUri);
            Port = ParsePort();
            _isTransactional = CheckForTransactionalHint(_rawUri);
            _isLocal = ()=>DetermineIfEndpointIsLocal(_rawUri);

            var sb = new StringBuilder();
            sb.Append(VHost);
            
            if(!VHost.EndsWith("/"))
                sb.Append('/');

            sb.Append(AddressType.ToString().ToLower());
            sb.Append('/');
            sb.Append(Queue);

            var builder = new UriBuilder("rabbitmq", Host, Port, sb.ToString())
                {
                    UserName = Username,
                    Password = Password,
                };

            _rebuiltUri = builder.Uri;
        }

        AddressType ParseExchange(Uri uri)
        {
            string[] bits = uri.LocalPath.Split('/');
            
            var e = bits[1];
            if (bits.Length == 4)
                e = bits[2];

            return e.Equals("exchange",StringComparison.InvariantCultureIgnoreCase) 
                ? RabbitMq.AddressType.Exchange : RabbitMq.AddressType.Queue;
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

        bool CheckForTransactionalHint(Uri rawUri)
        {
			return rawUri.Query.GetValueFromQueryString("tx", false);
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
            var path = address.PathAndQuery;
            var user = "guest";
            var password = "guest";
            if(!address.UserInfo.IsEmpty())
            {
                var parts = address.UserInfo.Split(':');
                user = parts[0];
                password = parts[1];
            }
            //vdir
            var vhost = "/";
            
            //type
            //path
            //query string

            var ub = new UriBuilder(address.Scheme, address.Host, port, path);
            return new RabbitMqAddress(address, ub.Uri, user, password, vhost);
            
        }

        public static RabbitMqAddress ParseForInbound(string address)
        {
            var m = _addressParser.Match(address);
            return new RabbitMqAddress(new Uri(address));
        }

        public static RabbitMqAddress ParseForOutbound(string address)
        {
            var m = _addressParser.Match(address);
            return new RabbitMqAddress(new Uri(address));
        }

        public static RabbitMqAddress ParseForDuplex(string address)
        {
            var m = _addressParser.Match(address);
            return new RabbitMqAddress(new Uri(address));
        }
    }

    public enum AddressType { Exchange, Queue }
}