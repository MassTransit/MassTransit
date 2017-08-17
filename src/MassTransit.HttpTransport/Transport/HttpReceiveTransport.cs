// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.HttpTransport.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline.Observables;
    using Topology;
    using Transports;
    using Util;


    public class HttpReceiveTransport :
        IReceiveTransport
    {
        readonly IHttpHost _host;
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveSettings _receiveSettings;
        readonly ReceiveTransportObservable _receiveTransportObservable;
        readonly IHttpReceiveEndpointTopology _topology;

        public HttpReceiveTransport(IHttpHost host, ReceiveSettings receiveSettings, IHttpReceiveEndpointTopology topology)
        {
            _host = host;
            _receiveSettings = receiveSettings;
            _topology = topology;

            _receiveObservable = new ReceiveObservable();
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

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "http");
            scope.Set(_host.Settings);
        }

        public ReceiveTransportHandle Start(IPipe<ReceiveContext> receivePipe)
        {
            var supervisor = new TaskSupervisor($"{TypeMetadataCache<HttpReceiveTransport>.ShortName} - {_host.Settings.GetInputAddress()}");

            IPipe<OwinHostContext> hostPipe = Pipe.New<OwinHostContext>(cxt =>
            {
                cxt.HttpConsumer(receivePipe, _host.Settings, _receiveSettings, _receiveObservable, _receiveTransportObservable, supervisor, _topology);
            });

            var hostTask = _host.OwinHostCache.Send(hostPipe, supervisor.StoppingToken);

            return new Handle(supervisor, hostTask);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _topology.PublishEndpointProvider.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _topology.SendEndpointProvider.ConnectSendObserver(observer);
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