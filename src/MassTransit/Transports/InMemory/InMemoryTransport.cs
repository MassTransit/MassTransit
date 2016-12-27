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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Events;
    using GreenPipes;
    using Logging;
    using Metrics;
    using Pipeline.Observables;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemoryTransport :
        IReceiveTransport,
        ISendTransport,
        IDisposable,
        IInMemoryTransport
    {
        static readonly ILog _log = Logger.Get<InMemoryTransport>();
        readonly Uri _inputAddress;
        readonly ITaskParticipant _participant;
        readonly IPublishEndpointProvider _publishEndpointProvider;
        readonly ReceiveObservable _receiveObservable;
        readonly LimitedConcurrencyLevelTaskScheduler _scheduler;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly SendObservable _sendObservable;
        readonly TaskSupervisor _supervisor;
        readonly IDeliveryTracker _tracker;
        readonly ReceiveTransportObservable _transportObservable;
        int _queueDepth;
        IPipe<ReceiveContext> _receivePipe;

        public InMemoryTransport(Uri inputAddress, int concurrencyLimit, ISendEndpointProvider sendEndpointProvider,
            IPublishEndpointProvider publishEndpointProvider)
        {
            _inputAddress = inputAddress;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpointProvider = publishEndpointProvider;

            _sendObservable = new SendObservable();
            _receiveObservable = new ReceiveObservable();
            _transportObservable = new ReceiveTransportObservable();

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _supervisor = new TaskSupervisor($"{TypeMetadataCache<InMemoryTransport>.ShortName} - {_inputAddress}");
            _participant = _supervisor.CreateParticipant($"{TypeMetadataCache<InMemoryTransport>.ShortName} - {_inputAddress}");

            _scheduler = new LimitedConcurrencyLevelTaskScheduler(concurrencyLimit);
        }

        public void Dispose()
        {
            _participant.SetComplete();

            TaskUtil.Await(() => _supervisor.Stop("Disposed"));

            TaskUtil.Await(() => _supervisor.Completed);
        }

        public int PendingDeliveryCount => _tracker.ActiveDeliveryCount;

        public long DeliveryCount => _tracker.DeliveryCount;

        public int MaxPendingDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        public int QueueDepth => _queueDepth;

        public void Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("transport");
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
            return _publishEndpointProvider.ConnectPublishObserver(observer);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new InMemorySendContext<T>(message, cancelSend);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                Guid messageId = context.MessageId ?? NewId.NextGuid();

                await _sendObservable.PreSend(context).ConfigureAwait(false);

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

                Interlocked.Increment(ref _queueDepth);

#pragma warning disable 4014
                Task.Factory.StartNew(() => DispatchMessage(transportMessage), _supervisor.StoppedToken, TaskCreationOptions.None, _scheduler);
#pragma warning restore 4014

                context.LogSent();

                await _sendObservable.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.LogFaulted(ex);

                await _sendObservable.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        async Task ISendTransport.Move(ReceiveContext context, IPipe<SendContext> pipe)
        {
            Guid messageId = GetMessageId(context);

            byte[] body;
            using (Stream bodyStream = context.GetBody())
            {
                body = await GetMessageBody(bodyStream).ConfigureAwait(false);
            }

            var messageType = "Unknown";
            InMemoryTransportMessage receivedMessage;
            if (context.TryGetPayload(out receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType.MediaType, messageType);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Factory.StartNew(() => DispatchMessage(transportMessage), _supervisor.StoppedToken, TaskCreationOptions.None, _scheduler);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        Task ISendTransport.Close()
        {
            // an in-memory send transport does not get disposed
            return TaskUtil.Completed;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        void HandleDeliveryComplete()
        {
        }

        async Task DispatchMessage(InMemoryTransportMessage message)
        {
            await _supervisor.Ready.ConfigureAwait(false);

            if (_supervisor.StoppedToken.IsCancellationRequested)
                return;

            if (_receivePipe == null)
                throw new ArgumentException("ReceivePipe not configured");

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveObservable, _sendEndpointProvider, _publishEndpointProvider);

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
                    Interlocked.Decrement(ref _queueDepth);

                    context.Dispose();
                }
            }
        }

        async Task<byte[]> GetMessageBody(Stream body)
        {
            using (var ms = new MemoryStream())
            {
                await body.CopyToAsync(ms).ConfigureAwait(false);

                return ms.ToArray();
            }
        }

        static Guid GetMessageId(ReceiveContext context)
        {
            object messageIdValue;
            return context.TransportHeaders.TryGetHeader("MessageId", out messageIdValue)
                ? new Guid(messageIdValue.ToString())
                : NewId.NextGuid();
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly ITaskParticipant _participant;
            readonly TaskSupervisor _supervisor;
            readonly InMemoryTransport _transport;

            public Handle(TaskSupervisor supervisor, ITaskParticipant participant, InMemoryTransport transport)
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