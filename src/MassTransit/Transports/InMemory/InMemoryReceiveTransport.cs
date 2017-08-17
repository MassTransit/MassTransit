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
    using Events;
    using Fabric;
    using GreenPipes;
    using Logging;
    using MassTransit.Topology;
    using Metrics;
    using Pipeline.Observables;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemoryReceiveTransport :
        IReceiveTransport,
        IInMemoryQueueConsumer
    {
        static readonly ILog _log = Logger.Get<InMemoryReceiveTransport>();
        readonly Uri _inputAddress;
        readonly ITaskParticipant _participant;
        readonly IInMemoryQueue _queue;
        readonly ReceiveObservable _receiveObservable;
        readonly TaskSupervisor _supervisor;
        readonly IReceiveEndpointTopology _topology;
        readonly IDeliveryTracker _tracker;
        readonly ReceiveTransportObservable _transportObservable;
        IPipe<ReceiveContext> _receivePipe;

        public InMemoryReceiveTransport(Uri inputAddress, IInMemoryQueue queue, IReceiveEndpointTopology topology)
        {
            _inputAddress = inputAddress;
            _queue = queue;
            _topology = topology;

            _receiveObservable = new ReceiveObservable();
            _transportObservable = new ReceiveTransportObservable();

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<InMemoryReceiveTransport>.ShortName} - {_inputAddress}");
            _participant = _supervisor.CreateParticipant($"{TypeMetadataCache<InMemoryReceiveTransport>.ShortName} - {_inputAddress}");
        }

        public async Task Consume(InMemoryTransportMessage message, CancellationToken cancellationToken)
        {
            await _supervisor.Ready.ConfigureAwait(false);

            if (_supervisor.StoppedToken.IsCancellationRequested)
                return;

            if (_receivePipe == null)
                throw new ArgumentException("ReceivePipe not configured");

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveObservable, _topology);

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

        ReceiveTransportHandle IReceiveTransport.Start(IPipe<ReceiveContext> receivePipe)
        {
            try
            {
                _receivePipe = receivePipe;

                _queue.ConnectConsumer(this);

                TaskUtil.Await(() => _transportObservable.Ready(new ReceiveTransportReadyEvent(_inputAddress)));

                _participant.SetReady();

                return new Handle(_supervisor, _participant, this);
            }
            catch (Exception exception)
            {
                _participant.SetNotReady(exception);
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
            return _topology.PublishEndpointProvider.ConnectPublishObserver(observer);
        }
        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _topology.SendEndpointProvider.ConnectSendObserver(observer);
        }

        void HandleDeliveryComplete()
        {
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly ITaskParticipant _participant;
            readonly TaskSupervisor _supervisor;
            readonly InMemoryReceiveTransport _transport;

            public Handle(TaskSupervisor supervisor, ITaskParticipant participant, InMemoryReceiveTransport transport)
            {
                _supervisor = supervisor;
                _participant = participant;
                _transport = transport;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                _participant.SetComplete();

                await _supervisor.Stop("Stopped", cancellationToken).ConfigureAwait(false);

                await _supervisor.Completed.ConfigureAwait(false);

                await _transport._transportObservable.Completed(new ReceiveTransportCompletedEvent(_transport._inputAddress,
                    _transport._tracker.GetDeliveryMetrics())).ConfigureAwait(false);
            }
        }
    }
}