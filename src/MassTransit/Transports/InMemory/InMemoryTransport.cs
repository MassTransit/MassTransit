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
    using Internals.Extensions;
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
        readonly int _concurrencyLimit;

        readonly Uri _inputAddress;
        readonly SendObservable _observers;
        readonly ReceiveObservable _receiveObservers;
        readonly QueuedTaskScheduler _scheduler;
        IPipe<ReceiveContext> _receivePipe;
        CancellationToken _stopToken;

        public InMemoryTransport(Uri inputAddress, int concurrencyLimit)
        {
            _inputAddress = inputAddress;
            _concurrencyLimit = concurrencyLimit;

            _observers = new SendObservable();
            _receiveObservers = new ReceiveObservable();

            _scheduler = new QueuedTaskScheduler(TaskScheduler.Default, concurrencyLimit);
        }

        public void Dispose()
        {
            _scheduler.Dispose();
        }

        public void Probe(ProbeContext context)
        {
            ProbeContext scope = context.CreateScope("transport");
            scope.Set(new
            {
                Address = _inputAddress,
            });
        }

        ReceiveTransportHandle IReceiveTransport.Start(IPipe<ReceiveContext> receivePipe)
        {
            var stopTokenSource = new CancellationTokenSource();

            _receivePipe = receivePipe;
            _stopToken = stopTokenSource.Token;

            return new Handle(stopTokenSource);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new InMemorySendContext<T>(message, cancelSend);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                Guid messageId = context.MessageId ?? NewId.NextGuid();

                _observers.NotifyPreSend(context);

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

                Task.Factory.StartNew(() => DispatchMessage(transportMessage), _stopToken, TaskCreationOptions.HideScheduler, _scheduler);

                context.DestinationAddress.LogSent(context.MessageId.HasValue ? context.MessageId.Value.ToString("N") : "",
                    TypeMetadataCache<T>.ShortName);

                _observers.NotifyPostSend(context);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("SEND FAULT: {0} {1} {2}", _inputAddress, context.MessageId, TypeMetadataCache<T>.ShortName));

                _observers.NotifySendFault(context, ex);

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

            string messageType = "Unknown";
            InMemoryTransportMessage receivedMessage;
            if (context.TryGetPayload(out receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType.MediaType, messageType);

            Task.Factory.StartNew(() => DispatchMessage(transportMessage), _stopToken, TaskCreationOptions.HideScheduler, _scheduler);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        async Task DispatchMessage(InMemoryTransportMessage message)
        {
            if (_stopToken.IsCancellationRequested)
                return;

            if (_receivePipe == null)
                throw new ArgumentNullException("ReceivePipe not configured");

            var context = new InMemoryReceiveContext(_inputAddress, message, _receiveObservers);

            try
            {
                _receiveObservers.NotifyPreReceive(context);

                _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                _receiveObservers.NotifyPostReceive(context);

                _inputAddress.LogReceived(message.MessageId.ToString("N"), message.MessageType);
            }
            catch (Exception ex)
            {
                message.DeliveryCount++;
                _log.Error(string.Format("RCV FAULT: {0}", message.MessageId), ex);

                _receiveObservers.NotifyReceiveFault(context, ex);
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
            readonly CancellationTokenSource _stop;

            public Handle(CancellationTokenSource cancellationTokenSource)
            {
                _stop = cancellationTokenSource;
            }

            void IDisposable.Dispose()
            {
                _stop.Cancel();
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                _stop.Cancel();
            }
        }
    }
}