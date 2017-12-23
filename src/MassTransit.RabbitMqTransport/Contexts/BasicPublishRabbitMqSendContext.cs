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
namespace MassTransit.RabbitMqTransport.Contexts
{
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class BasicPublishRabbitMqSendContext<T> :
        BaseSendContext<T>,
        RabbitMqSendContext<T>
        where T : class
    {
        public BasicPublishRabbitMqSendContext(IBasicProperties basicProperties, string exchange, T message, CancellationToken cancellationToken)
            : base(message, cancellationToken)
        {
            BasicProperties = basicProperties;

            AwaitAck = true;

            RoutingKey = "";

            Exchange = exchange;
        }

        public bool Mandatory { get; set; }
        public string Exchange { get; }
        public string RoutingKey { get; set; }
        public IBasicProperties BasicProperties { get; }
        public bool AwaitAck { get; set; }
    }
}