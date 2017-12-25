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
namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;


    /// <summary>
    /// A receive endpoint is called by the receive transport to push messages to consumers.
    /// The receive endpoint is where the initial deserialization occurs, as well as any additional
    /// filters on the receive context. 
    /// </summary>
    public class ReceiveEndpoint :
        IReceiveEndpointControl
    {
        readonly ReceiveEndpointObservable _observers;
        readonly IReceivePipe _receivePipe;
        readonly IReceiveTransport _receiveTransport;
        ConnectHandle _handle;

        public ReceiveEndpoint(IReceiveTransport receiveTransport, IReceivePipe receivePipe)
        {
            _receiveTransport = receiveTransport;
            _receivePipe = receivePipe;

            _observers = new ReceiveEndpointObservable();

            _handle = receiveTransport.ConnectReceiveTransportObserver(new Observer(this));
        }

        ReceiveEndpointHandle IReceiveEndpointControl.Start()
        {
            var transportHandle = _receiveTransport.Start(_receivePipe);

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
            return _observers.Connect(observer);
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
            return _receivePipe.ConsumePipe.ConnectConsumePipe(pipe);
        }

        ConnectHandle IRequestPipeConnector.ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
        {
            return _receivePipe.ConsumePipe.ConnectRequestPipe(requestId, pipe);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveTransport.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveTransport.ConnectSendObserver(observer);
        }


        public class Observer :
            IReceiveTransportObserver
        {
            readonly ReceiveEndpoint _endpoint;

            public Observer(ReceiveEndpoint endpoint)
            {
                _endpoint = endpoint;
            }

            public Task Ready(ReceiveTransportReady ready)
            {
                return _endpoint._observers.Ready(new ReceiveEndpointReadyEvent(ready.InputAddress, _endpoint));
            }

            public Task Completed(ReceiveTransportCompleted completed)
            {
                return _endpoint._observers.Completed(new ReceiveEndpointCompletedEvent(completed.InputAddress,
                    completed.DeliveryCount, completed.ConcurrentDeliveryCount, _endpoint));
            }

            public Task Faulted(ReceiveTransportFaulted faulted)
            {
                return _endpoint._observers.Faulted(new ReceiveEndpointFaultedEvent(faulted.InputAddress, faulted.Exception, _endpoint));
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