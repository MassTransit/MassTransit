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
namespace MassTransit.HttpTransport.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using Transports;


    public class HttpReceiveTransport :
        Supervisor,
        IReceiveTransport
    {
        readonly IHttpHost _host;
        readonly IPipe<HttpHostContext> _hostPipe;
        readonly HttpReceiveEndpointContext _receiveEndpointContext;

        public HttpReceiveTransport(IHttpHost host, HttpReceiveEndpointContext receiveEndpointContext, IPipe<HttpHostContext> hostPipe)
        {
            _host = host;
            _receiveEndpointContext = receiveEndpointContext;
            _hostPipe = hostPipe;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Add("type", "http");
            scope.Add("inputAddress", _receiveEndpointContext.InputAddress);
        }

        public ReceiveTransportHandle Start()
        {
            Task.Factory.StartNew(() => _host.HttpHostContextSupervisor.Send(_hostPipe, Stopping), CancellationToken.None, TaskCreationOptions.None,
                TaskScheduler.Default);

            return new Handle(this);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _receiveEndpointContext.ConnectReceiveTransportObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpointContext.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpointContext.ConnectSendObserver(observer);
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly IAgent _agent;

            public Handle(IAgent agent)
            {
                _agent = agent;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await _agent.Stop("Stop Receive Transport", cancellationToken).ConfigureAwait(false);
            }
        }
    }
}