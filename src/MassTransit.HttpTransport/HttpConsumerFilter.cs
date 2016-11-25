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
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using Hosting;
    using Logging;
    using MassTransit.Pipeline;
    using Pipeline;
    using Util;


    public class HttpConsumerFilter :
        IFilter<OwinHostContext>
    {
        static readonly ILog _log = Logger.Get<HttpConsumerFilter>();
        readonly IMessageSerializer _messageSerializer;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly ISendPipe _sendPipe;
        readonly HttpHostSettings _settings;
        readonly ITaskSupervisor _supervisor;

        readonly IReceiveTransportObserver _transportObserver;

        public HttpConsumerFilter(IPipe<ReceiveContext> receivePipe,
            IReceiveObserver receiveObserver,
            IReceiveTransportObserver transportObserver,
            ITaskSupervisor supervisor,
            HttpHostSettings settings,
            ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider,
            IMessageSerializer messageSerializer,
            ISendPipe sendPipe)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _transportObserver = transportObserver;
            _supervisor = supervisor;
            _settings = settings;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _messageSerializer = messageSerializer;
            _sendPipe = sendPipe;
        }

        public void Probe(ProbeContext context)
        {
            //no-op
        }

        public async Task Send(OwinHostContext context, IPipe<OwinHostContext> next)
        {
            var inputAddress = context.HostSettings.GetInputAddress();

            using (var scope = _supervisor.CreateScope($"{TypeMetadataCache<HttpConsumerFilter>.ShortName} - {inputAddress}", () => TaskUtil.Completed))
            {
                var controller = new HttpConsumerAction(_receiveObserver, _settings, _receivePipe, scope, _sendEndpointProvider, _publishEndpointProvider,
                    _messageSerializer, _sendPipe);

                context.StartHttpListener(controller);

                await scope.Ready.ConfigureAwait(false);

                await _transportObserver.Ready(new ReceiveTransportReadyEvent(inputAddress)).ConfigureAwait(false);

                scope.SetReady();

                try
                {
                    await scope.Completed.ConfigureAwait(false);
                }
                finally
                {
                    context.StopHttpListener();

                    HttpConsumerMetrics metrics = controller;
                    await _transportObserver.Completed(new ReceiveTransportCompletedEvent(inputAddress, metrics)).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Consumer {0} received, {1} concurrent", metrics.DeliveryCount, metrics.ConcurrentDeliveryCount);
                    }
                }
            }
        }
    }
}