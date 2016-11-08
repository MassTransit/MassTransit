// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Pipeline;


    /// <summary>
    /// Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHost :
        IInMemoryHost,
        ISendTransportProvider,
        IBusHostControl
    {
        readonly Uri _baseUri = new Uri("loopback://localhost/");
        readonly int _concurrencyLimit;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        readonly ConcurrentDictionary<string, InMemoryTransport> _transports;
        ISendEndpointProvider _sendEndpointProvider;
        IPublishEndpointProvider _publishEndpointProvider;

        public InMemoryHost(int concurrencyLimit)
        {
            _concurrencyLimit = concurrencyLimit;

            _transports = new ConcurrentDictionary<string, InMemoryTransport>(StringComparer.OrdinalIgnoreCase);
            _receiveEndpoints = new ReceiveEndpointCollection();
        }

        public IInMemoryReceiveEndpointFactory ReceiveEndpointFactory { private get; set; }

        public IEnumerable<Uri> TransportAddresses
        {
            get { return _transports.Keys.Select(x => new Uri(_baseUri, x)); }
        }

        public IReceiveEndpointCollection ReceiveEndpoints => _receiveEndpoints;

        public async Task<HostHandle> Start()
        {
            HostReceiveEndpointHandle[] handles = await ReceiveEndpoints.StartEndpoints().ConfigureAwait(false);

            return new Handle(this, handles);
        }

        public bool Matches(Uri address)
        {
            return address.Scheme.Equals(_baseUri.Scheme, StringComparison.OrdinalIgnoreCase)
                && address.Host.Equals(_baseUri.Host, StringComparison.OrdinalIgnoreCase);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "InMemory"
            });

            foreach (KeyValuePair<string, InMemoryTransport> transport in _transports)
            {
                var transportScope = scope.CreateScope("queue");
                transportScope.Set(new
                {
                    Name = transport.Key
                });
            }

            _receiveEndpoints.Probe(scope);
        }

        public IReceiveTransport GetReceiveTransport(string queueName, int concurrencyLimit, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider)
        {
            if (concurrencyLimit <= 0)
                concurrencyLimit = _concurrencyLimit;

            if (_sendEndpointProvider == null)
                _sendEndpointProvider = sendEndpointProvider;
            if (_publishEndpointProvider == null)
                _publishEndpointProvider = publishEndpointProvider;

            return _transports.GetOrAdd(queueName, name => new InMemoryTransport(new Uri(_baseUri, name), concurrencyLimit, sendEndpointProvider, publishEndpointProvider));
        }

        public Task<HostReceiveEndpointHandle> ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure)
        {
            if (ReceiveEndpointFactory == null)
                throw new ConfigurationException("The receive endpoint factory was not specified");

            ReceiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return _receiveEndpoints.Start(queueName);
        }

        public Uri Address => _baseUri;

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receiveEndpoints.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            var queueName = address.AbsolutePath;
            if (queueName.StartsWith("/"))
                queueName = queueName.Substring(1);

            return GetTransport(queueName);
        }

        public IInMemoryTransport GetTransport(string queueName)
        {
            return _transports.GetOrAdd(queueName, name =>
            {
                if (_sendEndpointProvider == null || _publishEndpointProvider == null)
                    throw new NotSupportedException("Okay, so somehow a send transport was requested first.");

                return new InMemoryTransport(new Uri(_baseUri, name), _concurrencyLimit, _sendEndpointProvider, _publishEndpointProvider);
            });
        }


        class Handle :
            BaseHostHandle
        {
            readonly InMemoryHost _host;

            public Handle(InMemoryHost host, HostReceiveEndpointHandle[] handles)
                : base(host, handles)
            {
                _host = host;
            }

            public override async Task Stop(CancellationToken cancellationToken)
            {
                await base.Stop(cancellationToken).ConfigureAwait(false);

                Parallel.ForEach(_host._transports.Values, x => x.Dispose());
            }
        }
    }
}