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
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Threading.Tasks;
    using RabbitMQ.Client;


    public class Published
    {
        ulong _publishTag;
        TaskCompletionSource<ulong> _source;

        public Published(string exchange, string routingKey, bool mandatory, bool immediate, IBasicProperties basicProperties, byte[] body,
            ulong publishTag)
        {
            _publishTag = publishTag;
            _source = new TaskCompletionSource<ulong>();
        }

        public Task Task
        {
            get { return _source.Task; }
        }

        public void Ack()
        {
            _source.TrySetResult(_publishTag);
        }

        public void Nack()
        {
            _source.TrySetException(new InvalidOperationException("Message was nacked"));
        }
    }
}