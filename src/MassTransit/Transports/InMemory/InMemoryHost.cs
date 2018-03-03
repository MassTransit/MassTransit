// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Builders;
    using Configuration;
    using Context;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using Logging;
    using MassTransit.Topology;
    using Pipeline;
    using Topology.Builders;
    using Util.Caching;


    /// <summary>
    /// Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHost :
        Supervisor,
        IInMemoryHostControl,
        ISendTransportProvider
    {
        static readonly ILog _log = Logger.Get<InMemoryHost>();
        readonly Uri _baseUri;
        readonly IIndex<string, InMemorySendTransport> _index;
        readonly IMessageFabric _messageFabric;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        readonly IInMemoryReceiveEndpointFactory _receiveEndpointFactory;

        public InMemoryHost(IInMemoryBusConfiguration busConfiguration, int concurrencyLimit, IHostTopology topology, Uri baseAddress = null)
        {
            Topology = topology;
            _baseUri = baseAddress ?? new Uri("loopback://localhost/");

            _messageFabric = new MessageFabric(concurrencyLimit);

            _receiveEndpoints = new ReceiveEndpointCollection();

            var cache = new GreenCache<InMemorySendTransport>(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24), () => DateTime.UtcNow);
            _index = cache.AddIndex("exchangeName", x => x.ExchangeName);

            _receiveEndpointFactory = new InMemoryReceiveEndpointFactory(busConfiguration);
        }

        public IReceiveEndpointCollection ReceiveEndpoints => _receiveEndpoints;

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            ReceiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        public async Task<HostHandle> Start()
        {
            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints();

            return new Handle(this, handles);
        }

        public bool Matches(Uri address)
        {
            return address.ToString().StartsWith(_baseUri.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "InMemory"
            });

            _messageFabric.Probe(scope);

            _receiveEndpoints.Probe(scope);
        }

        public IReceiveTransport GetReceiveTransport(string queueName, IReceivePipe receivePipe, ReceiveEndpointContext topology)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating receive transport for queue: {0}", queueName);

            var queue = _messageFabric.GetQueue(queueName);

            var skippedExchange = _messageFabric.GetExchange($"{queueName}_skipped");
            var errorExchange = _messageFabric.GetExchange($"{queueName}_error");

            var transport = new InMemoryReceiveTransport(new Uri(_baseUri, queueName), queue, receivePipe, errorExchange, skippedExchange, topology);
            Add(transport);

            return transport;
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure = null)
        {
            _receiveEndpointFactory.CreateReceiveEndpoint(queueName, configure);

            return _receiveEndpoints.Start(queueName);
        }

        public Uri Address => _baseUri;

        public IHostTopology Topology { get; }

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

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpoints.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpoints.ConnectSendObserver(observer);
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            var queueName = address.AbsolutePath.Split('/').Last();

            return await _index.Get(queueName, async key =>
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Creating send transport for exchange: {0}", queueName);

                var exchange = _messageFabric.GetExchange(queueName);

                var transport = new InMemorySendTransport(exchange);

                return transport;
            });
        }

        public IInMemoryExchange GetExchange(Uri address)
        {
            var queueName = address.AbsolutePath.Split('/').Last();

            var exchange = _messageFabric.GetExchange(queueName);

            return exchange;
        }

        public IInMemoryPublishTopologyBuilder CreatePublishTopologyBuilder(
            PublishEndpointTopologyBuilder.Options options = PublishEndpointTopologyBuilder.Options.MaintainHierarchy)
        {
            return new PublishEndpointTopologyBuilder(_messageFabric, options);
        }

        public IInMemoryConsumeTopologyBuilder CreateConsumeTopologyBuilder()
        {
            return new InMemoryConsumeTopologyBuilder(_messageFabric);
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
            }
        }
    }
}