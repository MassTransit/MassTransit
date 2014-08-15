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


    public class InMemoryReceiveTransport :
        IReceiveTransport,
        ISendTransport

    {
        static readonly ILog _log = Logger.Get<InMemoryReceiveTransport>();

        readonly Uri _inputAddress;
        BlockingCollection<InMemoryTransportMessage> _collection;
        ConcurrentQueue<InMemoryTransportMessage> _queue;

        public InMemoryReceiveTransport(Uri inputAddress)
        {
            _inputAddress = inputAddress;

            _queue = new ConcurrentQueue<InMemoryTransportMessage>();
            _collection = new BlockingCollection<InMemoryTransportMessage>(_queue);
        }

        public async Task Start(IPipe<ReceiveContext> receivePipe, CancellationToken cancellationToken)
        {
            Task receiveTask = Task.Run(() =>
            {
                using (CancellationTokenRegistration registration = cancellationToken.Register(() => _collection.CompleteAdding()))
                {
                    IEnumerable<InMemoryTransportMessage> consumingEnumerable = _collection.GetConsumingEnumerable();
                    Parallel.ForEach(consumingEnumerable, async message =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        var context = new InMemoryReceiveContext(_inputAddress, message);

                        try
                        {
                            await receivePipe.Send(context);
                        }
                        catch (Exception ex)
                        {
                            message.DeliveryCount++;
//                            _collection.Add(message, cancellationToken);
                        }
                    });
                }
            }, cancellationToken);

            await receiveTask;
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            var context = new InMemorySendContext<T>(message, cancellationToken);

            await pipe.Send(context);

            var transportMessage = new InMemoryTransportMessage(context.MessageId ?? NewId.NextGuid(), context.Body, context.ContentType.MediaType);

            _collection.Add(transportMessage, cancellationToken);
        }
    }
}