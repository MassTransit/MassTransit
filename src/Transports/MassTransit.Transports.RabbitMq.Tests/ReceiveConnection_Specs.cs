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
namespace MassTransit.Transports.RabbitMq.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Magnum.Extensions;
    using MassTransit.Pipeline;
    using NUnit.Framework;
    using Policies;
    using RabbitMQ.Client;
    using Serialization;


    [TestFixture]
    public class Reconnecting_a_consumer
    {
        [Test]
        public async void Should_handle_a_connection_shutdown()
        {
            var connectionFactory = new ConnectionFactory
            {
                VirtualHost = "speed",
                HostName = "localhost"
            };

            var testPipe = new TestReceivePipe();

            IRetryPolicy retryPolicy = Retry.Exponential(1.Seconds(), 60.Seconds(), 2.Seconds());
            var connectionMaker = new RabbitMqConnector(connectionFactory, retryPolicy);

            IReceiveTransport transport = new RabbitMqReceiveTransport(connectionMaker, Retry.None, new RabbitMqReceiveSettings
            {
                QueueName = "input",
                ExchangeName = "fast",
                AutoDelete = true,
                Durable = false,
                Exclusive = false,
                PrefetchCount = 100,
            });

            var receiveCancellationToken = new CancellationTokenSource();

            Task receiveTask = transport.Start(testPipe, receiveCancellationToken.Token);

            await Task.Delay(250);

            using (IConnection connection = connectionFactory.CreateConnection())
            using (var sendModel = new HaModel(connection.CreateModel()))
            {
                // this will kill the connection above, forcing it to reconnect to the server
                sendModel.QueueDelete("input");

                await Task.Delay(500);

//                var sendToTransport = new RabbitMqSendTransport(sendModel, "fast");
//                var sendSerializer = new JsonSendMessageSerializer(JsonMessageSerializer.Serializer);
//                ISendEndpoint sendToEndpoint = new SendEndpoint(sendToTransport, sendSerializer, new Uri("rabbitmq://localhost/speed/fast"),
//                    new Uri("rabbitmq://localhost/speed/input"));
//
//                var message = new TestMessage("Joe", "American Way", 27);
//
//                // now we send and ensure the message was received, showing the consumer reconnected and queue was recreated
//                await sendToEndpoint.Send(message);
//
//                Assert.IsTrue(testPipe.Task.Wait(10.Seconds()));
            }

            receiveCancellationToken.Cancel();
            receiveTask.Wait(10.Seconds());
        }


        public class TestMessage
        {
            public TestMessage(string name, string address, int count)
            {
                Count = count;
                Address = address;
                Name = name;
            }

            public TestMessage()
            {
            }

            public string Name { get; private set; }
            public string Address { get; private set; }
            public int Count { get; private set; }
        }


        class TestReceivePipe :
            IReceivePipe
        {
            readonly TaskCompletionSource<ReceiveContext> _source;

            public TestReceivePipe()
            {
                _source = new TaskCompletionSource<ReceiveContext>();
            }

            public Task Task
            {
                get { return _source.Task; }
            }

            public async Task Send(ReceiveContext context)
            {
                await Task.Yield();

                _source.TrySetResult(context);
            }

            public bool Inspect(IPipeInspector inspector)
            {
                return true;
            }
        }
    }
}