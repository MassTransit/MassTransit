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
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Pipeline;


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

        public InMemoryTransport(Uri inputAddress)
        {
            _inputAddress = inputAddress;

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

        async Task IReceiveTransport.Start(IPipe<ReceiveContext> receivePipe, CancellationToken stopReceive)
        {
            Task receiveTask = Task.Run(() =>
            {
                using (RegisterShutdown(stopReceive))
                {
                    IEnumerable<InMemoryTransportMessage> consumingEnumerable = _collection.GetConsumingEnumerable();

                    Parallel.ForEach(consumingEnumerable, async message =>
                    {
                        if (stopReceive.IsCancellationRequested)
                            return;

                        var context = new InMemoryReceiveContext(_inputAddress, message);

                        try
                        {
                            await receivePipe.Send(context);
                        }
                        catch (Exception ex)
                        {
                            message.DeliveryCount++;
                            _log.Error(string.Format("Receive Fault: {0}", message.MessageId), ex);

                            _collection.Add(message, stopReceive);
                        }
                    });
                }
            }, stopReceive);

            await receiveTask;
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new InMemorySendContext<T>(message, cancelSend);

            await pipe.Send(context);

            Guid messageId = context.MessageId ?? NewId.NextGuid();
            var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType);

            _collection.Add(transportMessage, cancelSend);
        }

        CancellationTokenRegistration RegisterShutdown(CancellationToken cancellationToken)
        {
            return cancellationToken.Register(() =>
            {
                // signal collection that no more messages will be added, ending it
                _collection.CompleteAdding();
            });
        }
    }
}