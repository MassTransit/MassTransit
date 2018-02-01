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
namespace MassTransit.Topology
{
    using System;
    using EndpointSpecifications;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;


    public abstract class ReceiveEndpointTopology :
        IReceiveEndpointTopology
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

        protected ReceiveEndpointTopology(IEndpointConfiguration configuration, Uri inputAddress, Uri hostAddress)
        {
            InputAddress = inputAddress;
            HostAddress = hostAddress;

            _send = configuration.Topology.Send;
            _publish = configuration.Topology.Publish;

            SendObservers = new SendObservable();
            PublishObservers = new PublishObservable();

            _sendPipe = new Lazy<ISendPipe>(() => configuration.Send.CreatePipe());
            _publishPipe = new Lazy<IPublishPipe>(() => configuration.Publish.CreatePipe());

            _serializer = new Lazy<IMessageSerializer>(() => configuration.Serialization.Serializer);
            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        protected IPublishPipe PublishPipe => _publishPipe.Value;
        protected ISendPipe SendPipe => _sendPipe.Value;
        protected IMessageSerializer Serializer => _serializer.Value;

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return PublishObservers.Connect(observer);
        }

        protected Uri HostAddress { get; }
        public Uri InputAddress { get; }

        ISendTopology IReceiveTopology.Send => _send;
        IPublishTopology IReceiveTopology.Publish => _publish;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;
        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        protected abstract ISendEndpointProvider CreateSendEndpointProvider();
        protected abstract IPublishEndpointProvider CreatePublishEndpointProvider();
    }
}