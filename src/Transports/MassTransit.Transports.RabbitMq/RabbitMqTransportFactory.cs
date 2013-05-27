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
    using System.Collections.Generic;
    using System.Linq;
    using Configuration.Builders;
    using Configuration.Configurators;
    using Exceptions;
    using Logging;
    using Magnum.Caching;
    using Magnum.Extensions;
    using RabbitMQ.Client;


    public class RabbitMqTransportFactory :
        ITransportFactory
    {
        readonly Cache<ConnectionFactory, ConnectionFactoryBuilder> _connectionFactoryBuilders;
        readonly Cache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>> _inboundConnections;
        readonly ILog _log = Logger.Get<RabbitMqTransportFactory>();
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly Cache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>> _outboundConnections;
        bool _disposed;

        public RabbitMqTransportFactory(
            IEnumerable<KeyValuePair<Uri, ConnectionFactoryBuilder>> connectionFactoryBuilders)
        {
            _inboundConnections = new ConcurrentCache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>>(
                new ConnectionFactoryEquality());

            _outboundConnections = new ConcurrentCache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>>(
                new ConnectionFactoryEquality());

            Dictionary<ConnectionFactory, ConnectionFactoryBuilder> builders = connectionFactoryBuilders
                .Select(x => new KeyValuePair<ConnectionFactory, ConnectionFactoryBuilder>(
                                 RabbitMqEndpointAddress.Parse(x.Key).ConnectionFactory, x.Value))
                .ToDictionary(x => x.Key, x => x.Value);

            _connectionFactoryBuilders = new ConcurrentCache<ConnectionFactory, ConnectionFactoryBuilder>(builders,
                new ConnectionFactoryEquality());

            _messageNameFormatter = new RabbitMqMessageNameFormatter();
        }

        public RabbitMqTransportFactory()
        {
            _inboundConnections = new ConcurrentCache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>>(
                new ConnectionFactoryEquality());
            _outboundConnections = new ConcurrentCache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>>(
                new ConnectionFactoryEquality());
            _connectionFactoryBuilders =
                new ConcurrentCache<ConnectionFactory, ConnectionFactoryBuilder>(new ConnectionFactoryEquality());
            _messageNameFormatter = new RabbitMqMessageNameFormatter();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public string Scheme
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("RabbitMQTransportFactory");

                return "rabbitmq";
            }
        }

        public IDuplexTransport BuildLoopback(ITransportSettings settings)
        {
            if (_disposed)
                throw new ObjectDisposedException("RabbitMQTransportFactory");

            RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

            var transport = new Transport(address, () => BuildInbound(settings), () => BuildOutbound(settings));

            return transport;
        }

        public IInboundTransport BuildInbound(ITransportSettings settings)
        {
            if (_disposed)
                throw new ObjectDisposedException("RabbitMQTransportFactory");

            RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

            EnsureProtocolIsCorrect(address.Uri);

            ConnectionHandler<RabbitMqConnection> connectionHandler = GetConnection(_inboundConnections, address);

            return new InboundRabbitMqTransport(address, connectionHandler, settings.PurgeExistingMessages,
                _messageNameFormatter);
        }

        public IOutboundTransport BuildOutbound(ITransportSettings settings)
        {
            if (_disposed)
                throw new ObjectDisposedException("RabbitMQTransportFactory");

            RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

            EnsureProtocolIsCorrect(address.Uri);

            ConnectionHandler<RabbitMqConnection> connectionHandler = GetConnection(_outboundConnections, address);

            return new OutboundRabbitMqTransport(address, connectionHandler, false);
        }

        public IOutboundTransport BuildError(ITransportSettings settings)
        {
            if (_disposed)
                throw new ObjectDisposedException("RabbitMQTransportFactory");

            RabbitMqEndpointAddress address = RabbitMqEndpointAddress.Parse(settings.Address.Uri);

            EnsureProtocolIsCorrect(address.Uri);

            ConnectionHandler<RabbitMqConnection> connection = GetConnection(_outboundConnections, address);

            return new OutboundRabbitMqTransport(address, connection, true);
        }

        public IMessageNameFormatter MessageNameFormatter
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("RabbitMQTransportFactory");

                return _messageNameFormatter;
            }
        }

        public IEndpointAddress GetAddress(Uri uri, bool transactional)
        {
            return RabbitMqEndpointAddress.Parse(uri);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _inboundConnections.Each(x => x.Dispose());
                _outboundConnections.Each(x => x.Dispose());
            }
            _inboundConnections.Clear();
            _outboundConnections.Clear();

            _disposed = true;
        }

        public int ConnectionCount()
        {
            return _inboundConnections.Count() + _outboundConnections.Count;
        }

        ConnectionHandler<RabbitMqConnection> GetConnection(
            Cache<ConnectionFactory, ConnectionHandler<RabbitMqConnection>> cache, IRabbitMqEndpointAddress address)
        {
            ConnectionFactory factory = SanitizeConnectionFactory(address);

            return cache.Get(factory, _ =>
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Creating RabbitMQ connection: {0}", address.Uri);

                    ConnectionFactoryBuilder builder = _connectionFactoryBuilders.Get(factory, __ =>
                        {
                            if (_log.IsDebugEnabled)
                                _log.DebugFormat("Using default configurator for connection: {0}", address.Uri);

                            var configurator = new ConnectionFactoryConfiguratorImpl(address);

                            return configurator.CreateBuilder();
                        });

                    ConnectionFactory connectionFactory = builder.Build();

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("RabbitMQ connection created: {0}:{1}/{2}", connectionFactory.HostName,
                            connectionFactory.Port, connectionFactory.VirtualHost);
                    }

                    var connection = new RabbitMqConnection(connectionFactory);
                    var connectionHandler = new ConnectionHandlerImpl<RabbitMqConnection>(connection);
                    return connectionHandler;
                });
        }

        ConnectionFactory SanitizeConnectionFactory(IRabbitMqEndpointAddress address)
        {
            ConnectionFactory factory = address.ConnectionFactory;

            foreach (ConnectionFactory builder in _connectionFactoryBuilders.GetAllKeys())
            {
                if (string.Equals(factory.VirtualHost, builder.VirtualHost)
                    && (string.Compare(factory.HostName, builder.HostName, StringComparison.OrdinalIgnoreCase) == 0)
                    && Equals(factory.Ssl, builder.Ssl)
                    && factory.Port == builder.Port)
                {
                    bool userNameMatch = string.IsNullOrEmpty(factory.UserName)
                                         || string.CompareOrdinal(factory.UserName, builder.UserName) == 0;
                    bool passwordMatch = string.IsNullOrEmpty(factory.Password)
                                         || string.CompareOrdinal(factory.UserName, builder.Password) == 0;

                    if (userNameMatch && passwordMatch)
                        return builder;
                }
            }

            return address.ConnectionFactory;
        }

        static void EnsureProtocolIsCorrect(Uri address)
        {
            if (address.Scheme != "rabbitmq")
            {
                throw new EndpointException(address,
                    "Address must start with 'rabbitmq' not '{0}'".FormatWith(address.Scheme));
            }
        }


        class ConnectionFactoryEquality :
            IEqualityComparer<ConnectionFactory>
        {
            public bool Equals(ConnectionFactory x, ConnectionFactory y)
            {
                return string.Equals(x.UserName, y.UserName)
                       && string.Equals(x.Password, y.Password)
                       && string.Equals(x.VirtualHost, y.VirtualHost)
                       && string.Equals(x.HostName, y.HostName)
                       && Equals(x.Ssl, y.Ssl)
                       && x.Port == y.Port;
            }

            public int GetHashCode(ConnectionFactory x)
            {
                unchecked
                {
                    int hashCode = (x.UserName != null
                                        ? x.UserName.GetHashCode()
                                        : 0);
                    hashCode = (hashCode * 397) ^ (x.Password != null
                                                       ? x.Password.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (x.VirtualHost != null
                                                       ? x.VirtualHost.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (x.Ssl != null
                                                       ? GetHashCode(x.Ssl)
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (x.HostName != null
                                                       ? x.HostName.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ x.Port;
                    return hashCode;
                }
            }

            bool Equals(SslOption x, SslOption y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(null, x))
                    return false;
                if (ReferenceEquals(null, y))
                    return false;

                return x.Version == y.Version
                       && x.Enabled.Equals(y.Enabled)
                       && string.Equals(x.CertPath, y.CertPath)
                       && string.Equals(x.CertPassphrase, y.CertPassphrase)
                       && Equals(x.Certs, y.Certs)
                       && string.Equals(x.ServerName, y.ServerName)
                       && x.AcceptablePolicyErrors == y.AcceptablePolicyErrors;
            }


            int GetHashCode(SslOption x)
            {
                unchecked
                {
                    var hashCode = (int)x.Version;
                    hashCode = (hashCode * 397) ^ x.Enabled.GetHashCode();
                    hashCode = (hashCode * 397) ^ (x.CertPath != null
                                                       ? x.CertPath.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (x.CertPassphrase != null
                                                       ? x.CertPassphrase.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (x.Certs != null
                                                       ? x.Certs.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (x.ServerName != null
                                                       ? x.ServerName.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (int)x.AcceptablePolicyErrors;
                    return hashCode;
                }
            }
        }
    }
}