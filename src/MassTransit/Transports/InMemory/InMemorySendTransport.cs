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
    using Contexts;
    using Fabric;
    using GreenPipes;
    using Pipeline.Observables;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemorySendTransport :
        ISendTransport
    {
        readonly IInMemoryExchange _exchange;
        readonly SendObservable _sendObservable;

        public InMemorySendTransport(IInMemoryExchange exchange)
        {
            _exchange = exchange;

            _sendObservable = new SendObservable();
        }

        public string ExchangeName => _exchange.Name;

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            var context = new InMemorySendContext<T>(message, cancellationToken);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                var messageId = context.MessageId ?? NewId.NextGuid();

                await _sendObservable.PreSend(context).ConfigureAwait(false);

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

                await _exchange.Send(transportMessage).ConfigureAwait(false);

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

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }
    }
}