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
namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Observables;
    using Pipeline;
    using Topology;
    using Transports;


    public class RabbitMqSendEndpointProvider :
        ISendEndpointProvider
    {
        readonly ISendEndpointCache<Uri> _cache;
        readonly BusHostCollection<RabbitMqHost> _hosts;
        readonly SendObservable _observers;
        readonly ISendPipe _sendPipe;
        readonly IMessageSerializer _serializer;
        readonly Uri _sourceAddress;

        public RabbitMqSendEndpointProvider(BusHostCollection<RabbitMqHost> hosts, SendObservable observers, IMessageSerializer serializer, Uri sourceAddress, ISendPipe sendPipe)
        {
            _hosts = hosts;
            _serializer = serializer;
            _sourceAddress = sourceAddress;
            _sendPipe = sendPipe;

            _cache = new SendEndpointCache<Uri>();
            _observers = observers;
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _cache.GetSendEndpoint(address, CreateSendEndpoint);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        Task<ISendEndpoint> CreateSendEndpoint(Uri address)
        {
            var sendTransport = GetSendTransport(address);

            var handle = sendTransport.ConnectSendObserver(_observers);

            return Task.FromResult<ISendEndpoint>(new SendEndpoint(sendTransport, _serializer, address, _sourceAddress, _sendPipe, handle));
        }

        ISendTransport GetSendTransport(Uri address)
        {
            var host = _hosts.GetHost(address);

            var settings = host.Topology.SendTopology.GetSendSettings(address);

            var brokerTopology = settings.GetBrokerTopology();

            IAgent<ModelContext> modelSource = GetModelSource(host);

            var configureTopologyFilter = new ConfigureTopologyFilter<SendSettings>(settings, brokerTopology);

            var transport = new RabbitMqSendTransport(modelSource, configureTopologyFilter, settings.ExchangeName);

            host.Add(transport);

            return transport;
        }

        protected virtual IAgent<ModelContext> GetModelSource(RabbitMqHost host)
        {
            return new RabbitMqModelCache(host, host.ConnectionCache);
        }
    }
}