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
namespace MassTransit.HttpTransport
{
    using System.Collections.Generic;
    using GreenPipes;
    using Hosting;
    using MassTransit.Pipeline;
    using Util;


    public class HttpConsumerPipeSpecification :
        IPipeSpecification<OwinHostContext>
    {
        readonly IMessageSerializer _messageSerializer;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly HttpHostSettings _settings;
        readonly ITaskSupervisor _supervisor;
        readonly IReceiveTransportObserver _transportObserver;

        public HttpConsumerPipeSpecification(HttpHostSettings settings,
            IPipe<ReceiveContext> receivePipe,
            IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver,
            ITaskSupervisor supervisor,
            ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider,
            IMessageSerializer messageSerializer,
            ISendPipe sendPipe)
        {
            _settings = settings;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _messageSerializer = messageSerializer;
            _sendPipe = sendPipe;
        }

        public void Apply(IPipeBuilder<OwinHostContext> builder)
        {
            builder.AddFilter(new HttpConsumerFilter(_receivePipe, _receiveObserver, _transportObserver, _supervisor, _settings, _sendEndpointProvider,
                _publishEndpointProvider, _messageSerializer, _sendPipe));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_settings == null)
                yield return this.Failure("Settings", "must not be null");
        }
    }
}