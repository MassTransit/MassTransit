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
namespace MassTransit.HttpTransport.Configuration.Builders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Transports;
    using Util;


    public class HttpReceiveTransport :
        IReceiveTransport
    {
        static readonly ILog _log = Logger.Get<HttpReceiveTransport>();

        readonly IHttpHost _host;
        readonly ReceiveEndpointObservable _receiveEndpointObservable;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveTransportObservable _receiveTransportObservable;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly IMessageSerializer _messageSerializer;
        readonly ISendPipe _sendPipe;

        public HttpReceiveTransport(IHttpHost host, ISendEndpointProvider sendEndpointProvider, IPublishEndpointProvider publishEndpointProvider, IMessageSerializer messageSerializer, ISendPipe sendPipe)
        {
            _host = host;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;
            _messageSerializer = messageSerializer;
            _sendPipe = sendPipe;

            _receiveObservable = new ReceiveObservable();
            _receiveEndpointObservable = new ReceiveEndpointObservable();
            _receiveTransportObservable = new ReceiveTransportObservable();
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _receiveTransportObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpointObservable.Connect(observer);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "HTTP");
            scope.Set(_host.Settings);
        }

        public ReceiveTransportHandle Start(IPipe<ReceiveContext> receivePipe)
        {
            var supervisor = new TaskSupervisor($"{TypeMetadataCache<HttpReceiveTransport>.ShortName} - {_host.Settings.GetInputAddress()}");

            IPipe<OwinHostContext> hostPipe = Pipe.New<OwinHostContext>(cxt =>
            {
                cxt.HttpConsumer(receivePipe, _host.Settings, _receiveObservable, _receiveTransportObservable, supervisor, _sendEndpointProvider, _publishEndpointProvider, _messageSerializer, _sendPipe);
            });

            var hostTask = _host.OwinHostCache.Send(hostPipe, supervisor.StoppingToken);

            return new Handle(supervisor, hostTask);
        }

        class Handle :
            ReceiveTransportHandle
        {
            readonly Task _connectionTask;
            readonly TaskSupervisor _supervisor;

            public Handle(TaskSupervisor supervisor, Task connectionTask)
            {
                _supervisor = supervisor;
                _connectionTask = connectionTask;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await _supervisor.Stop("Stop Receive Transport", cancellationToken).ConfigureAwait(false);

                await _connectionTask.ConfigureAwait(false);
            }
        }
    }
}