// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Magnum;
    using Magnum.Extensions;
    using RabbitMQ.Client;
    using Util;


    public class RabbitMqEndpointAddress :
        IRabbitMqEndpointAddress
    {
        const string FormatErrorMsg =
            "The path can be empty, or a sequence of these characters: letters, digits, hyphen, underscore, period, or colon.";

        static readonly string LocalMachineName = Environment.MachineName.ToLowerInvariant();
        static readonly Regex _regex = new Regex(@"^[A-Za-z0-9\-_\.:]+$");
        readonly ConnectionFactory _connectionFactory;
        readonly bool _isHighAvailable;
        readonly bool _isTransactional;
        readonly string _name;
        readonly Uri _uri;
        Func<bool> _isLocal;
        ushort _prefetch;
        int _ttl;

        public RabbitMqEndpointAddress(Uri uri, ConnectionFactory connectionFactory, string name)
        {
            _uri = GetSanitizedUri(uri).Uri;

            _connectionFactory = connectionFactory;
            _name = name;

            _isTransactional = uri.Query.GetValueFromQueryString("tx", false);
            _isLocal = () => DetermineIfEndpointIsLocal(uri);
            _isHighAvailable = uri.Query.GetValueFromQueryString("ha", false);
            _ttl = uri.Query.GetValueFromQueryString("ttl", 0);
            _prefetch = uri.Query.GetValueFromQueryString("prefetch", (ushort)Math.Max(Environment.ProcessorCount, 10));
        }

        public ConnectionFactory ConnectionFactory
        {
            get { return _connectionFactory; }
        }

        public string Name
        {
            get { return _name; }
        }

        public ushort PrefetchCount
        {
            get { return _prefetch; }
        }

        public IRabbitMqEndpointAddress ForQueue(string name)
        {
            string uri = _uri.ToString();
            uri = uri.Remove(uri.Length - _name.Length);
            return new RabbitMqEndpointAddress(new Uri(uri).AppendToPath(name), _connectionFactory, name);
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public bool IsLocal
        {
            get { return _isLocal(); }
        }

        public bool IsTransactional
        {
            get { return _isTransactional; }
        }

        public IDictionary QueueArguments()
        {
            var ht = new Hashtable();

            if (_isHighAvailable)
                ht.Add("x-ha-policy", "all");
            if (_ttl > 0)
                ht.Add("x-message-ttl", _ttl);

            return ht.Keys.Count == 0
                       ? null
                       : ht;
        }

        public void SetTtl(TimeSpan ttl)
        {
            _ttl = ttl.Milliseconds;
        }

        public void SetPrefetchCount(ushort count)
        {
            _prefetch = count;
        }

        static UriBuilder GetSanitizedUri(Uri uri)
        {
            var uriPath = new Uri(uri.GetLeftPart(UriPartial.Path));
            var builder = new UriBuilder(uriPath.Scheme, uriPath.Host, uriPath.Port, uriPath.PathAndQuery);
            return builder;
        }

        public override string ToString()
        {
            return _uri.ToString();
        }

        bool DetermineIfEndpointIsLocal(Uri uri)
        {
            string hostName = uri.Host;
            bool local = string.CompareOrdinal(hostName, ".") == 0 ||
                         string.Compare(hostName, "localhost", StringComparison.OrdinalIgnoreCase) == 0 ||
                         string.Compare(uri.Host, LocalMachineName, StringComparison.OrdinalIgnoreCase) == 0;

            Interlocked.Exchange(ref _isLocal, () => local);

            return local;
        }

        public static RabbitMqEndpointAddress Parse(string address)
        {
            return Parse(new Uri(address));
        }

        public static RabbitMqEndpointAddress Parse(Uri address)
        {
            Guard.AgainstNull(address, "address");

            if (string.Compare("rabbitmq", address.Scheme, true) != 0)
                throw new RabbitMqAddressException("The invalid scheme was specified: " + address.Scheme ?? "(null)");

            var connectionFactory = new ConnectionFactory
                {
                    HostName = address.Host,
                    UserName = "",
                    Password = "",
                };

            if (address.IsDefaultPort)
                connectionFactory.Port = 5672;
            else if (!address.IsDefaultPort)
                connectionFactory.Port = address.Port;

            if (!address.UserInfo.IsEmpty())
            {
                if (address.UserInfo.Contains(":"))
                {
                    string[] parts = address.UserInfo.Split(':');
                    connectionFactory.UserName = parts[0];
                    connectionFactory.Password = parts[1];
                }
                else
                    connectionFactory.UserName = address.UserInfo;
            }

            string name = address.AbsolutePath.Substring(1);
            string[] pathSegments = name.Split('/');
            if (pathSegments.Length == 2)
            {
                connectionFactory.VirtualHost = pathSegments[0];
                name = pathSegments[1];
            }

            ushort heartbeat = address.Query.GetValueFromQueryString("heartbeat", connectionFactory.RequestedHeartbeat);
            connectionFactory.RequestedHeartbeat = heartbeat;

            VerifyQueueOrExchangeNameIsLegal(name);

            return new RabbitMqEndpointAddress(address, connectionFactory, name);
        }

        static void VerifyQueueOrExchangeNameIsLegal(string path)
        {
            Match match = _regex.Match(path);

            if (!match.Success)
                throw new RabbitMqAddressException(FormatErrorMsg);
        }
    }
}