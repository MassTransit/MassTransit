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
    using MassTransit.Topology;
    using Metrics;
    using Pipeline.Observables;
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
        readonly ReceiveObservable _receiveObservable;
        readonly ReceiveEndpointContext _topology;
        readonly IDeliveryTracker _tracker;
        readonly ReceiveTransportObservable _transportObservable;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IErrorTransport _errorTransport;
        readonly IDeadLetterTransport _deadLetterTransport;

        public InMemoryReceiveTransport(Uri inputAddress, IInMemoryQueue queue, IPipe<ReceiveContext> receivePipe, IInMemoryExchange errorExchange,
            IInMemoryExchange deadLetterExchange, ReceiveEndpointContext topology)
        {
            _inputAddress = inputAddress;
            _queue = queue;
            _topology = topology;
            _receivePipe = receivePipe;

            _errorTransport = new InMemoryMessageErrorTransport(errorExchange);
            _deadLetterTransport = new InMemoryMessageDeadLetterTransport(deadLetterExchange);

            _receiveObservable = new ReceiveObservable();
            _transportObservable = new ReceiveTransportObservable();

            _tracker = new DeliveryTracker(HandleDeliveryComplete);
        }

        public async Task Consume(InMemoryTransportMessage message, CancellationToken cancellationToken)
        {
            await Ready.ConfigureAwait(false);
            if (IsStopped)
                return;
            
            if (_receivePipe == null)
                throw new ArgumentException("ReceivePipe not configured");

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveObservable, _topology);
            context.GetOrAddPayload(() => _errorTransport);
            context.GetOrAddPayload(() => _deadLetterTransport);

            using (_tracker.BeginDelivery())
            {
                try
                {
                    await _receiveObservable.PreReceive(context).ConfigureAwait(false);

                    await _receivePipe.Send(context).ConfigureAwait(false);

                    await context.CompleteTask.ConfigureAwait(false);

                    await _receiveObservable.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _receiveObservable.ReceiveFault(context, ex).ConfigureAwait(false);

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
            scope.Set(new
            {
                Address = _inputAddress
            });
        }

        ReceiveTransportHandle IReceiveTransport.Start()
        {
            try
            {
                _queue.ConnectConsumer(this);

                TaskUtil.Await(() => _transportObservable.Ready(new ReceiveTransportReadyEvent(_inputAddress)));

                SetReady();

                return new Handle(this);
            }
            catch (Exception exception)
            {
                SetNotReady(exception);
                throw;
            }
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _transportObservable.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _topology.ConnectPublishObserver(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _topology.ConnectSendObserver(observer);
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
                
                await _transport._transportObservable.Completed(new ReceiveTransportCompletedEvent(_transport._inputAddress,
                    _transport._tracker.GetDeliveryMetrics())).ConfigureAwait(false);
            }
        }
    }
}