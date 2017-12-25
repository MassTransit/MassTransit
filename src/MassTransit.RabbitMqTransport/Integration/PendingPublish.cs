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
namespace MassTransit.RabbitMqTransport.Integration
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// A pending BasicPublish to RabbitMQ, waiting for an ACK/NAK from the broker
    /// </summary>
    public class PendingPublish
    {
        readonly ConnectionContext _connectionContext;
        readonly string _exchange;
        readonly ulong _publishTag;
        readonly TaskCompletionSource<ulong> _source;

        public PendingPublish(ConnectionContext connectionContext, string exchange, ulong publishTag)
        {
            _connectionContext = connectionContext;
            _exchange = exchange;
            _publishTag = publishTag;
            _source = new TaskCompletionSource<ulong>();
        }

        public Task Task => _source.Task;

        Uri DestinationAddress => _connectionContext.Topology.GetDestinationAddress(_exchange);

        public void Ack()
        {
            _source.TrySetResult(_publishTag);
        }

        public void Nack()
        {
            _source.TrySetException(new PublishNackException(DestinationAddress, "The message was nacked by RabbitMQ"));
        }

        public void PublishNotConfirmed()
        {
            _source.TrySetException(new MessageNotConfirmedException(DestinationAddress));
        }

        public void PublishReturned(ushort code, string text)
        {
            _source.TrySetException(new PublishReturnedException(DestinationAddress, $"The message was returned by RabbitMQ: {code}-{text}"));
        }
    }
}