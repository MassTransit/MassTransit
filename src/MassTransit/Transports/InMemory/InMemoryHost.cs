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
    using Definition;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Caching;
    using Logging;
    using MassTransit.Configurators;
    using Topology.Builders;


    /// <summary>
    ///     Caches InMemory transport instances so that they are only created and used once
    /// </summary>
    public class InMemoryHost :
        BaseHost,
        IInMemoryHostControl
    {
        static readonly ILog _log = Logger.Get<InMemoryHost>();

        readonly IInMemoryHostConfiguration _hostConfiguration;
        readonly IIndex<string, InMemorySendTransport> _index;
        readonly IMessageFabric _messageFabric;

        public InMemoryHost(IInMemoryHostConfiguration hostConfiguration, int concurrencyLimit)
            : base(hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;

            _messageFabric = new MessageFabric(concurrencyLimit);

            var cacheSettings = new CacheSettings(10000, TimeSpan.FromMinutes(1), TimeSpan.FromHours(24));

            var cache = new GreenCache<InMemorySendTransport>(cacheSettings);
            _index = cache.AddIndex("exchangeName", x => x.ExchangeName);
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

            var transport = new InMemoryReceiveTransport(new Uri(_hostConfiguration.HostAddress, queueName), queue, receiveEndpointContext);
            Add(transport);

            return transport;
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            var queueName = definition.GetEndpointName(endpointNameFormatter ?? DefaultEndpointNameFormatter.Instance);

            return ConnectReceiveEndpoint(queueName, x => x.Apply(definition, configureEndpoint));
        }

        public override HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IInMemoryReceiveEndpointConfigurator> configure = null)
        {
            var configuration = _hostConfiguration.CreateReceiveEndpointConfiguration(queueName);

            configure?.Invoke(configuration.Configurator);

            BusConfigurationResult.CompileResults(configuration.Validate());

            configuration.Build();

            return ReceiveEndpoints.Start(queueName);
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
            }).ConfigureAwait(false);
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

        protected override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "InMemory",
                BaseAddress = _hostConfiguration.HostAddress
            });

            _messageFabric.Probe(context);
        }

        public IInMemoryExchange GetExchange(Uri address)
        {
            var queueName = address.AbsolutePath.Split('/').Last();

            var exchange = _messageFabric.GetExchange(queueName);

            return exchange;
        }
    }
}
