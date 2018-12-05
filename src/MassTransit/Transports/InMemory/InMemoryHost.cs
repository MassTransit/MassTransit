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
    using System.Threading.Tasks;
    using Builders;
    using Configuration;
    using Context;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Caching;
    using Logging;
    using MassTransit.Configurators;
    using MassTransit.Topology;
    using Pipeline;
    using Topology.Builders;


    /// <summary>
    ///     Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHost :
        Supervisor,
        IInMemoryHostControl
    {
        static readonly ILog _log = Logger.Get<InMemoryHost>();

        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly IIndex<string, InMemorySendTransport> _index;
        readonly IMessageFabric _messageFabric;
        readonly IReceiveEndpointCollection _receiveEndpoints;

        public InMemoryHost(IInMemoryHostConfiguration hostConfiguration, int concurrencyLimit, IHostTopology topology, Uri baseAddress = null)
        {
            _hostConfiguration = hostConfiguration;
            Topology = topology;
            Address = baseAddress ?? new Uri("loopback://localhost/");

            _messageFabric = new MessageFabric(concurrencyLimit);

            _receiveEndpoints = new ReceiveEndpointCollection();
            Add(_receiveEndpoints);

            var cacheSettings = new CacheSettings(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24));

            var cache = new GreenCache<InMemorySendTransport>(cacheSettings);
            _index = cache.AddIndex("exchangeName", x => x.ExchangeName);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            _receiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        public async Task<HostHandle> Start()
        {
            HostReceiveEndpointHandle[] handles = _receiveEndpoints.StartEndpoints();

            return new StartHostHandle(this, handles);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");
            scope.Set(new
            {
                Type = "InMemory",
                BaseAddress = Address
            });

            _messageFabric.Probe(scope);

            _receiveEndpoints.Probe(scope);
        }

        public IReceiveTransport GetReceiveTransport(string queueName, ReceiveEndpointContext receiveEndpointContext)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating receive transport for queue: {0}", queueName);

            var queue = _messageFabric.GetQueue(queueName);

            IDeadLetterTransport deadLetterTransport = new InMemoryMessageDeadLetterTransport(_messageFabric.GetExchange($"{queueName}_skipped"));
            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);

            IErrorTransport errorTransport = new InMemoryMessageErrorTransport(_messageFabric.GetExchange($"{queueName}_error"));
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            var transport = new InMemoryReceiveTransport(new Uri(Address, queueName), queue, receiveEndpointContext);
            Add(transport);

            return transport;
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return _receiveEndpoints.Start(queueName);
        }

        public Uri Address { get; }

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

        public int TransportConcurrencyLimit
        {
            set => _messageFabric.ConcurrencyLimit = value;
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

        public IInMemoryExchange GetExchange(Uri address)
        {
            var queueName = address.AbsolutePath.Split('/').Last();

            var exchange = _messageFabric.GetExchange(queueName);

            return exchange;
        }
    }
}