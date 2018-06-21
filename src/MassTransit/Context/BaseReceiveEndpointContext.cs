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
namespace MassTransit.Context
{
    using System;
    using Configuration;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
    using Topology;


    public abstract class BaseReceiveEndpointContext :
        ReceiveEndpointContext
    {
        readonly IPublishTopologyConfigurator _publish;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly Lazy<IPublishPipe> _publishPipe;
        readonly ISendTopologyConfigurator _send;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly Lazy<ISendPipe> _sendPipe;
        readonly Lazy<IMessageSerializer> _serializer;
        protected readonly PublishObservable PublishObservers;
        protected readonly SendObservable SendObservers;

        protected BaseReceiveEndpointContext(IReceiveEndpointConfiguration configuration, ReceiveObservable receiveObservers,
            ReceiveTransportObservable transportObservers, ReceiveEndpointObservable endpointObservers)
        {
            InputAddress = configuration.InputAddress;
            HostAddress = configuration.HostAddress;

            _send = configuration.Topology.Send;
            _publish = configuration.Topology.Publish;

            SendObservers = new SendObservable();
            PublishObservers = new PublishObservable();
            EndpointObservers = endpointObservers;

            ReceiveObservers = receiveObservers;
            TransportObservers = transportObservers;

            _sendPipe = new Lazy<ISendPipe>(() => configuration.Send.CreatePipe());
            _publishPipe = new Lazy<IPublishPipe>(() => configuration.Publish.CreatePipe());

            _serializer = new Lazy<IMessageSerializer>(() => configuration.Serialization.Serializer);
            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        protected IPublishPipe PublishPipe => _publishPipe.Value;
        protected ISendPipe SendPipe => _sendPipe.Value;
        protected IMessageSerializer Serializer => _serializer.Value;

        protected Uri HostAddress { get; }

        public ReceiveObservable ReceiveObservers { get; }
        public ReceiveTransportObservable TransportObservers { get; }
        public ReceiveEndpointObservable EndpointObservers { get; }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return PublishObservers.Connect(observer);
        }

        public Uri InputAddress { get; }

        ISendTopology ReceiveEndpointContext.Send => _send;
        IPublishTopology ReceiveEndpointContext.Publish => _publish;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        protected abstract ISendEndpointProvider CreateSendEndpointProvider();
        protected abstract IPublishEndpointProvider CreatePublishEndpointProvider();
    }
}