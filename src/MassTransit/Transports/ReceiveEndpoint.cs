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
namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Events;
    using GreenPipes;
    using Pipeline;


    /// <summary>
    /// A receive endpoint is called by the receive transport to push messages to consumers.
    /// The receive endpoint is where the initial deserialization occurs, as well as any additional
    /// filters on the receive context. 
    /// </summary>
    public class ReceiveEndpoint :
        IReceiveEndpointControl
    {
        readonly IReceivePipe _receivePipe;
        readonly IReceiveTransport _receiveTransport;
        ConnectHandle _handle;
        readonly ReceiveEndpointContext _context;

        public ReceiveEndpoint(IReceiveTransport receiveTransport, IReceivePipe receivePipe, ReceiveEndpointContext context)
        {
            _context = context;
            _receiveTransport = receiveTransport;
            _receivePipe = receivePipe;

            _handle = receiveTransport.ConnectReceiveTransportObserver(new Observer(this, context.EndpointObservers));
        }

        ReceiveEndpointContext IReceiveEndpoint.Context => _context;

        ReceiveEndpointHandle IReceiveEndpointControl.Start()
        {
            var transportHandle = _receiveTransport.Start();

            return new Handle(transportHandle);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _receiveTransport.Probe(context);

            _receivePipe.Probe(context);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveTransport.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _context.EndpointObservers.Connect(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receivePipe.ConnectConsumeObserver(observer);
        }

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receivePipe.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumePipeConnector.ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            return _receivePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _receivePipe.ConnectRequestPipe(requestId, pipe);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveTransport.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveTransport.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _context.SendEndpointProvider.GetSendEndpoint(address);
        }

        public IPublishEndpoint CreatePublishEndpoint(Uri sourceAddress, ConsumeContext context = null)
        {
            return _context.PublishEndpointProvider.CreatePublishEndpoint(sourceAddress, context);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>(T message)
            where T : class
        {
            return _context.PublishEndpointProvider.GetPublishSendEndpoint(message);
        }


        class Observer :
            IReceiveTransportObserver
        {
            readonly ReceiveEndpoint _endpoint;
            readonly IReceiveEndpointObserver _observer;

            public Observer(ReceiveEndpoint endpoint, IReceiveEndpointObserver observer)
            {
                _endpoint = endpoint;
                _observer = observer;
            }

            public Task Ready(ReceiveTransportReady ready)
            {
                return _observer.Ready(new ReceiveEndpointReadyEvent(ready.InputAddress, _endpoint));
            }

            public Task Completed(ReceiveTransportCompleted completed)
            {
                return _observer.Completed(new ReceiveEndpointCompletedEvent(completed.InputAddress, completed.DeliveryCount, completed.ConcurrentDeliveryCount,
                    _endpoint));
            }

            public Task Faulted(ReceiveTransportFaulted faulted)
            {
                return _observer.Faulted(new ReceiveEndpointFaultedEvent(faulted.InputAddress, faulted.Exception, _endpoint));
            }
        }


        class Handle :
            ReceiveEndpointHandle
        {
            readonly ReceiveTransportHandle _transportHandle;

            public Handle(ReceiveTransportHandle transportHandle)
            {
                _transportHandle = transportHandle;
            }

            Task ReceiveEndpointHandle.Stop(CancellationToken cancellationToken)
            {
                return _transportHandle.Stop(cancellationToken);
            }
        }
    }
}