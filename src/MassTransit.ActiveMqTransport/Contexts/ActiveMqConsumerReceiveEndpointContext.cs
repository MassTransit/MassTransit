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
namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using Configuration;
    using Context;
    using MassTransit.Pipeline.Observables;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class ActiveMqConsumerReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ActiveMqReceiveEndpointContext
    {
        readonly IActiveMqReceiveEndpointConfiguration _configuration;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;
        readonly Lazy<IPublishTransportProvider> _publishTransportProvider;
        readonly IActiveMqPublishTopology _publishTopology;

        public ActiveMqConsumerReceiveEndpointContext(IActiveMqReceiveEndpointConfiguration configuration, BrokerTopology brokerTopology,
            ReceiveObservable receiveObservers, ReceiveTransportObservable transportObservers, ReceiveEndpointObservable endpointObservers)
            : base(configuration, receiveObservers, transportObservers, endpointObservers)
        {
            _configuration = configuration;
            BrokerTopology = brokerTopology;

            _publishTopology = configuration.Topology.Publish;

            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
            _publishTransportProvider = new Lazy<IPublishTransportProvider>(CreatePublishTransportProvider);
        }

        public BrokerTopology BrokerTopology { get; }

        public ISendEndpointProvider CreateSendEndpointProvider(ReceiveContext receiveContext)
        {
            return SendEndpointProvider;
        }

        public IPublishEndpointProvider CreatePublishEndpointProvider(ReceiveContext receiveContext)
        {
            return PublishEndpointProvider;
        }

        ISendTransportProvider CreateSendTransportProvider()
        {
            return new SendTransportProvider(_configuration.BusConfiguration);
        }

        IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new PublishTransportProvider(_configuration.Host, _publishTopology);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new SendEndpointProvider(_sendTransportProvider.Value, SendObservers, Serializer, InputAddress, SendPipe);
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new PublishEndpointProvider(_publishTransportProvider.Value, _configuration.HostAddress, PublishObservers, SendObservers, Serializer,
                InputAddress, PublishPipe, _publishTopology);
        }
    }
}