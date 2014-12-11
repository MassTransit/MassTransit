// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using Logging;
    using Pipeline;
    using Subscriptions;
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

        readonly BlockingCollection<InMemoryTransportMessage> _collection;
        readonly Uri _inputAddress;
        readonly Connectable<ISendObserver> _observers;

        public InMemoryTransport(Uri inputAddress)
        {
            _inputAddress = inputAddress;

            _observers = new Connectable<ISendObserver>();

            var queue = new ConcurrentQueue<InMemoryTransportMessage>();
            _collection = new BlockingCollection<InMemoryTransportMessage>(queue);
        }

        public void Dispose()
        {
            if (_collection != null)
                _collection.Dispose();
        }

        public Uri InputAddress
        {
            get { return _inputAddress; }
        }

        async Task<ReceiveTransportHandle> IReceiveTransport.Start(IPipe<ReceiveContext> receivePipe, CancellationToken cancellationToken)
        {
            var handle = new Handle(this);

            Task receiveTask = Task.Run(() =>
            {
                _log.DebugFormat("Starting InMemory Transport: {0}", _inputAddress);
                using (RegisterShutdown(handle.StopToken))
                {
                    Parallel.ForEach(GetConsumingPartitioner(_collection), async message =>
                    {
                        if (handle.StopToken.IsCancellationRequested)
                            return;

                        var context = new InMemoryReceiveContext(_inputAddress, message);

                        try
                        {
                            await receivePipe.Send(context);

                            _log.DebugFormat("RECV: {0} {1}", _inputAddress, message.MessageId);

                        }
                        catch (Exception ex)
                        {
                            message.DeliveryCount++;
                            _log.Error(string.Format("Receive Fault: {0}", message.MessageId), ex);

                            _collection.Add(message, handle.StopToken);
                        }
                    });

                    handle.Stopped();
                }
            }, handle.StopToken);

            return handle;
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new InMemorySendContext<T>(message, cancelSend);

            try
            {
                await pipe.Send(context);

                Guid messageId = context.MessageId ?? NewId.NextGuid();

                await _observers.ForEach(x => x.PreSend(context));

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType);

                _collection.Add(transportMessage, cancelSend);

                _log.DebugFormat("SEND: {0} {1} {2}", _inputAddress, transportMessage.MessageId, TypeMetadataCache<T>.ShortName);

                await _observers.ForEach(x => x.PostSend(context));
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("SEND FAULT: {0} {1} {2}", _inputAddress, context.MessageId, TypeMetadataCache<T>.ShortName));

                _observers.ForEach(x => x.SendFault(context, ex))
                    .Wait(cancelSend);
            }
        }

        async Task ISendTransport.Move(ReceiveContext context)
        {
            Guid messageId = GetMessageId(context);

            byte[] body = await GetMessageBody(context.Body);

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType.MediaType);

            _collection.Add(transportMessage, context.CancellationToken);
        }

        public ConnectHandle Connect(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        async Task<byte[]> GetMessageBody(Stream body)
        {
            using (var ms = new MemoryStream())
            {
                await body.CopyToAsync(ms);

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

        CancellationTokenRegistration RegisterShutdown(CancellationToken cancellationToken)
        {
            return cancellationToken.Register(() =>
            {
                // signal collection that no more messages will be added, ending it
                _collection.CompleteAdding();
            });
        }

        Partitioner<T> GetConsumingPartitioner<T>(BlockingCollection<T> collection)
        {
            return new BlockingCollectionPartitioner<T>(collection);
        }


        class BlockingCollectionPartitioner<T> :
            Partitioner<T>
        {
            readonly BlockingCollection<T> _collection;

            internal BlockingCollectionPartitioner(BlockingCollection<T> collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");
                _collection = collection;
            }

            public override bool SupportsDynamicPartitions
            {
                get { return true; }
            }

            public override IList<IEnumerator<T>> GetPartitions(int partitionCount)
            {
                if (partitionCount < 1)
                    throw new ArgumentOutOfRangeException("partitionCount");

                IEnumerable<T> dynamicPartitioner = GetDynamicPartitions();

                return Enumerable.Range(0, partitionCount).Select(_ => dynamicPartitioner.GetEnumerator()).ToArray();
            }

            public override IEnumerable<T> GetDynamicPartitions()
            {
                return _collection.GetConsumingEnumerable();
            }
        }


        class Handle :
            ReceiveTransportHandle
        {
            readonly CancellationTokenSource _stop;
            readonly IReceiveTransport _transport;
            readonly TaskCompletionSource<bool> _stopped; 

            public Handle(IReceiveTransport transport)
            {
                _transport = transport;
                _stop = new CancellationTokenSource();
                _stopped = new TaskCompletionSource<bool>();
            }

            void IDisposable.Dispose()
            {
                _stop.Cancel();
            }

            IReceiveTransport ReceiveTransportHandle.Transport
            {
                get { return _transport; }
            }

            async Task ReceiveTransportHandle.Stop(CancellationToken cancellationToken)
            {
                _stop.Cancel();

                await _stopped.Task.WithCancellation(cancellationToken);
            }

            public CancellationToken StopToken
            {
                get { return _stop.Token; }
            }

            public void Stopped()
            {
                _stopped.TrySetResult(true);
            }
        }
    }
}