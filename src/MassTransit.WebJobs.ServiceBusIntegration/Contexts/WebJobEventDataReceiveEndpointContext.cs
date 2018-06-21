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
namespace MassTransit.WebJobs.ServiceBusIntegration.Contexts
{
    using System;
    using System.Threading;
    using Context;
    using Logging;
    using MassTransit.Configuration;
    using Microsoft.Azure.WebJobs;
    using Pipeline.Observables;
    using Topology;
    using Transports;


    public class WebJobEventDataReceiveEndpointContext :
        BaseReceiveEndpointContext
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;
        readonly ILog _log;
        readonly IPublishTopology _publishTopology;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;

        public WebJobEventDataReceiveEndpointContext(IReceiveEndpointConfiguration configuration, ILog log, IBinder binder,
            CancellationToken cancellationToken, ReceiveObservable receiveObservers, ReceiveTransportObservable transportObservers,
            ReceiveEndpointObservable endpointObservers)
            : base(configuration, receiveObservers, transportObservers, endpointObservers)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;
            _log = log;

            _publishTopology = configuration.Topology.Publish;

            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new SendEndpointProvider(_sendTransportProvider.Value, SendObservers, Serializer, InputAddress, SendPipe);
        }

        ISendTransportProvider CreateSendTransportProvider()
        {
            return new EventHubAttributeSendTransportProvider(_binder, _log, _cancellationToken);
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            var publishTransportProvider = new EventHubAttributePublishTransportProvider(_sendTransportProvider.Value);

            return new PublishEndpointProvider(publishTransportProvider, HostAddress, PublishObservers, SendObservers, Serializer, InputAddress, PublishPipe,
                _publishTopology);
        }
    }
}