// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Pipeline;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemoryTransport :
        IReceiveTransport,
        ISendTransport,
        IDisposable
    {
        static readonly ILog _log = Logger.Get<InMemoryTransport>();
        readonly ReceiveEndpointObservable _endpointObservable;
        readonly Uri _inputAddress;
        readonly ReceiveObservable _receiveObservable;
        readonly QueuedTaskScheduler _scheduler;
        readonly SendObservable _sendObservable;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        IPipe<ReceiveContext> _receivePipe;
        TaskSupervisor _supervisor;

        public InMemoryTransport(Uri inputAddress, int concurrencyLimit)
        {
            _inputAddress = inputAddress;

            _sendObservable = new SendObservable();
            _receiveObservable = new ReceiveObservable();
            _endpointObservable = new ReceiveEndpointObservable();
            _supervisor = new TaskSupervisor();

            _scheduler = new QueuedTaskScheduler(TaskScheduler.Default, concurrencyLimit);
        }

        public void Dispose()
        {
            TaskUtil.Await(() => _supervisor.Stop("Disposed"));

            TaskUtil.Await(() => _supervisor.Completed);

            _scheduler.Dispose();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("transport");
            scope.Set(new
            {
                Address = _inputAddress
            });
        }

        ReceiveTransportHandle IReceiveTransport.Start(IPipe<ReceiveContext> receivePipe)
        {
            _receivePipe = receivePipe;

            TaskUtil.Await(() => _endpointObservable.Ready(new Ready(_inputAddress)));

            return new Handle(_supervisor, this);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservable.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservable.Connect(observer);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new InMemorySendContext<T>(message, cancelSend);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                var messageId = context.MessageId ?? NewId.NextGuid();

                await _sendObservable.PreSend(context).ConfigureAwait(false);

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

#pragma warning disable 4014
                Task.Factory.StartNew(() => DispatchMessage(transportMessage), _supervisor.StopToken, TaskCreationOptions.HideScheduler, _scheduler);
#pragma warning restore 4014

                context.DestinationAddress.LogSent(context.MessageId?.ToString("N") ?? "", TypeMetadataCache<T>.ShortName);

                await _sendObservable.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error($"SEND FAULT: {_inputAddress} {context.MessageId} {TypeMetadataCache<T>.ShortName}", ex);

                await _sendObservable.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        async Task ISendTransport.Move(ReceiveContext context, IPipe<SendContext> pipe)
        {
            var messageId = GetMessageId(context);

            byte[] body;
            using (var bodyStream = context.GetBody())
            {
                body = await GetMessageBody(bodyStream);
            }

            var messageType = "Unknown";
            InMemoryTransportMessage receivedMessage;
            if (context.TryGetPayload(out receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType.MediaType, messageType);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Factory.StartNew(() => DispatchMessage(transportMessage), _supervisor.StopToken, TaskCreationOptions.HideScheduler, _scheduler);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        async Task DispatchMessage(InMemoryTransportMessage message)
        {
            if (_supervisor.StopToken.IsCancellationRequested)
                return;

            if (_receivePipe == null)
                throw new ArgumentException("ReceivePipe not configured");

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveObservable);

            Interlocked.Increment(ref _deliveryCount);

            var current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            try
            {
                await _receiveObservable.PreReceive(context).ConfigureAwait(false);

                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                await _receiveObservable.PostReceive(context).ConfigureAwait(false);

                _inputAddress.LogReceived(message.MessageId.ToString("N"), message.MessageType);
            }
            catch (Exception ex)
            {
                _log.Error($"RCV FAULT: {message.MessageId}", ex);

                await _receiveObservable.ReceiveFault(context, ex).ConfigureAwait(false);

                message.DeliveryCount++;
            }
            finally
            {
                Interlocked.Decrement(ref _currentPendingDeliveryCount);
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
            readonly TaskSupervisor _supervisor;
            readonly InMemoryTransport _transport;

            public Handle(TaskSupervisor supervisor, InMemoryTransport transport)
            {
                _supervisor = supervisor;
                _transport = transport;
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                await _supervisor.Stop("Stopped").ConfigureAwait(false);

                await _supervisor.Completed.ConfigureAwait(false);

                await _transport._endpointObservable.Completed(new Completed(_transport._inputAddress, _transport._deliveryCount,
                    _transport._maxPendingDeliveryCount)).ConfigureAwait(false);
            }
        }


        class Ready :
            ReceiveEndpointReady
        {
            public Ready(Uri inputAddress)
            {
                InputAddress = inputAddress;
            }

            public Uri InputAddress { get; }
        }


        class Completed :
            ReceiveEndpointCompleted
        {
            public Completed(Uri inputAddress, long deliveryCount, long concurrentDeliveryCount)
            {
                InputAddress = inputAddress;
                DeliveryCount = deliveryCount;
                ConcurrentDeliveryCount = concurrentDeliveryCount;
            }

            public Uri InputAddress { get; }
            public long DeliveryCount { get; }
            public long ConcurrentDeliveryCount { get; }
        }
    }
}