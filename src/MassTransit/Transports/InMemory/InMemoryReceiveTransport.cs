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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Events;
    using Fabric;
    using GreenPipes;
    using GreenPipes.Agents;
    using Metrics;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemoryReceiveTransport :
        Agent,
        IReceiveTransport,
        IInMemoryQueueConsumer
    {
        readonly Uri _inputAddress;
        readonly IInMemoryQueue _queue;
        readonly ReceiveEndpointContext _receiveEndpointContext;
        readonly IDeliveryTracker _tracker;

        public InMemoryReceiveTransport(Uri inputAddress, IInMemoryQueue queue, ReceiveEndpointContext receiveEndpointContext)
        {
            _inputAddress = inputAddress;
            _queue = queue;
            _receiveEndpointContext = receiveEndpointContext;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);
        }

        public async Task Consume(InMemoryTransportMessage message, CancellationToken cancellationToken)
        {
            await Ready.ConfigureAwait(false);
            if (IsStopped)
                return;

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveEndpointContext);

            using (_tracker.BeginDelivery())
            {
                try
                {
                    await _receiveEndpointContext.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                    await _receiveEndpointContext.ReceivePipe.Send(context).ConfigureAwait(false);

                    await context.ReceiveCompleted.ConfigureAwait(false);

                    await _receiveEndpointContext.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _receiveEndpointContext.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);

                    message.DeliveryCount++;
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("inMemoryReceiveTransport");
            scope.Set(new {Address = _inputAddress});
        }

        ReceiveTransportHandle IReceiveTransport.Start()
        {
            try
            {
                _queue.ConnectConsumer(this);

                void NotifyReady()
                {
                    _receiveEndpointContext.TransportObservers.Ready(new ReceiveTransportReadyEvent(_inputAddress));

                    SetReady();
                }

                Task.Factory.StartNew(NotifyReady, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

                return new Handle(this);
            }
            catch (Exception exception)
            {
                SetNotReady(exception);
                throw;
            }
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

        void HandleDeliveryComplete()
        {
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly InMemoryReceiveTransport _transport;

            public Handle(InMemoryReceiveTransport transport)
            {
                _transport = transport;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await _transport.Stop("Stop", cancellationToken).ConfigureAwait(false);

                var completed = new ReceiveTransportCompletedEvent(_transport._inputAddress, _transport._tracker.GetDeliveryMetrics());

                await _transport._receiveEndpointContext.TransportObservers.Completed(completed).ConfigureAwait(false);
            }
        }
    }
}
