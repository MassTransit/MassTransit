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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Pipeline;
    using Policies;
    using RabbitMQ.Client;


    public class RabbitMqReceiveTransport :
        IReceiveTransport
    {
        readonly ConnectionFactory _connectionFactory;
        readonly IRetryPolicy _retryPolicy;

        public RabbitMqReceiveTransport(ConnectionFactory connectionFactory, IRetryPolicy retryPolicy)
        {
            _connectionFactory = connectionFactory;
            _retryPolicy = retryPolicy;
        }

        public async Task Start(IPipe<ReceiveContext> pipe, CancellationToken cancellationToken)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    var inputAddress = new Uri("rabbitmq://localhost/speed/input");

                    var consumer = new RabbitMqBasicConsumer(model, inputAddress, pipe);

                    model.QueueDeclare("input", false, false, true, new Dictionary<string, object>());
                    model.QueuePurge("input");

                    model.ExchangeDeclare("fast", ExchangeType.Fanout, false, true, new Dictionary<string, object>());
                    model.QueueBind("input", "fast", "");

                    model.BasicQos(0, 100, false);
                    model.BasicConsume("input", false, consumer);


                }
            }
        }
    }
}