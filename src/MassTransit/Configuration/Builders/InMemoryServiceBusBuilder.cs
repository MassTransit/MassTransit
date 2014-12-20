// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Builders
{
    using System;
    using System.Linq;
    using Pipeline;
    using Pipeline.Pipes;
    using Transports;


    public class InMemoryServiceBusBuilder :
        ServiceBusBuilderBase,
        IInMemoryServiceBusBuilder
    {
        readonly Uri _inputAddress;
        readonly IReceiveTransportProvider _receiveTransportProvider;
        readonly ISendTransportProvider _sendTransportProvider;

        public InMemoryServiceBusBuilder(Uri inputAddress, IReceiveTransportProvider receiveTransportProvider,
            ISendTransportProvider sendTransportProvider)
        {
            if (inputAddress == null)
                throw new ArgumentNullException("inputAddress");
            if (receiveTransportProvider == null)
                throw new ArgumentNullException("receiveTransportProvider");
            if (sendTransportProvider == null)
                throw new ArgumentNullException("sendTransportProvider");

            _inputAddress = inputAddress;
            _receiveTransportProvider = receiveTransportProvider;
            _sendTransportProvider = sendTransportProvider;
        }

        public ISendTransportProvider SendTransportProvider
        {
            get { return _sendTransportProvider; }
        }

        public IReceiveTransportProvider ReceiveTransportProvider
        {
            get { return _receiveTransportProvider; }
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            var provider = new InMemorySendEndpointProvider(_inputAddress, _sendTransportProvider, MessageSerializer);

            return new SendEndpointCache(provider);
        }

        protected override IPublishEndpoint CreatePublishEndpoint()
        {
            return new InMemoryPublishEndpoint(SendEndpointProvider, _sendTransportProvider);
        }

        public virtual IBusControl Build()
        {
            IConsumePipe consumePipe = ReceiveEndpoints.Where(x => x.InputAddress.Equals(_inputAddress))
                .Select(x => x.ConsumePipe).FirstOrDefault() ?? new ConsumePipe();

            return new MassTransitBus(_inputAddress, consumePipe, SendEndpointProvider, PublishEndpoint, ReceiveEndpoints);
        }
    }
}