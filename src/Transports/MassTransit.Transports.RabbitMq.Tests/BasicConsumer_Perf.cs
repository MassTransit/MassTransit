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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Magnum.Extensions;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Serialization;
    using Serialization.Custom;


    [TestFixture]
    public class BasicConsumer_Perf
    {
        public interface ITestMessage
        {
            string Name { get; }
            string Address { get; }
            int Count { get; }
        }


        public class TestMessage :
            ITestMessage
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


        public class Person
        {
            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public Person()
            {
            }

            public string FirstName { get; private set; }
            public string LastName { get; private set; }
        }


        [Test]
        public void Including_full_deserialization()
        {
            var connectionFactory = new ConnectionFactory
            {
                VirtualHost = "speed",
                HostName = "localhost"
            };

            const int limit = 100000;
            int count = 0;
            int failureCount = 0;
            int redeliviered = 0;
            var completed = new TaskCompletionSource<bool>();
            int processingTime = 0;

            var deserializer = new JsonMessageDeserializer(JsonMessageSerializer.Deserializer);

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    var consumer = new RabbitMqBasicConsumer(model, new Uri("rabbitmq://localhost/speed/input"), context =>
                        {
                            return Task.Run(() =>
                                {
                                    if (context.Redelivered)
                                        Interlocked.Increment(ref redeliviered);

                                    ConsumeContext consumeContext = deserializer.Deserialize(context);

                                   // var person = consumeContext.Headers.Get<Person>("person");

                                    ConsumeContext<ITestMessage> messageContext;
                                    if (!consumeContext.TryGetMessage(out messageContext))
                                        throw new SerializationException("The message type was not supported");

                                    Interlocked.Add(ref processingTime, (int) messageContext.ReceiveContext.ElapsedTime.TotalMilliseconds);

                                    var number = Interlocked.Increment(ref count);
                                    if (number == limit)
                                        completed.TrySetResult(true);
                                });
                        });


                    model.QueueDeclare("input", false, false, true, new Dictionary<string, object>());
                    model.QueuePurge("input");

                    model.ExchangeDeclare("fast", ExchangeType.Fanout, false, true, new Dictionary<string, object>());
                    model.QueueBind("input", "fast", "");

                    model.BasicQos(0, 100, false);
                    model.BasicConsume("input", false, consumer);

                    var message = new TestMessage("Joe", "American Way", 27);

                    byte[] body;
                    using (var output = new MemoryStream())
                    using (var nonClosingStream = new NonClosingStream(output))
                    using (var writer = new StreamWriter(nonClosingStream))
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        var envelope = new Envelope(message, message.GetType().GetMessageTypes());
                        envelope.SourceAddress = "rabbitmq://localhost/speed/fast";
                        envelope.DestinationAddress = "rabbitmq://localhost/speed/input";
                        envelope.MessageId = NewId.NextGuid().ToString();
                        envelope.Headers["person"] = new Person("Joe", "Blow");

                        jsonWriter.Formatting = Formatting.Indented;

                        JsonMessageSerializer.Serializer.Serialize(jsonWriter, envelope);

                        jsonWriter.Flush();
                        writer.Flush();

                        body = output.ToArray();
                    }

                    //Console.WriteLine(Encoding.UTF8.GetString(body));

                    using (var sendModel = new HaModel(connection.CreateModel()))
                    {
                        var sendToTransport = new RabbitMqSendToTransport(sendModel, "fast");
                        var sendSerializer = new JsonSendMessageSerializer(JsonMessageSerializer.Serializer);
                        var sendToEndpoint = new SendEndpoint(sendToTransport, sendSerializer, new Uri("rabbitmq://localhost/speed/fast"));
                        Stopwatch timer = Stopwatch.StartNew();
                        var tasks = Enumerable.Range(0, limit)
                            .Select(x =>
                                {
                                    return sendToEndpoint.Send(message);
//                                    var properties = sendModel.CreateBasicProperties();
//                                    return sendModel.BasicPublishAsync("fast", "", properties, body);
                                }).AsParallel().ToArray();

                        completed.Task.Wait(10.Seconds());
                        timer.Stop();

                        Task.WaitAll(tasks, 10.Seconds());

                        Console.WriteLine("Elapsed time for {0} messages: {1}ms", limit, timer.ElapsedMilliseconds);
                        Console.WriteLine("Messages per second: {0}", limit * 1000L / timer.ElapsedMilliseconds);
                        Console.WriteLine("Processing time per message: {0}ms", processingTime / count);

                        Console.WriteLine("Messages: {0}", count);
                        Console.WriteLine("Redelivered: {0}", redeliviered);
                    }
                }
            }
        }

        [Test]
        public void No_jacket_required_raw_in_memory_speed_no_serialization()
        {
            var connectionFactory = new ConnectionFactory
            {
                VirtualHost = "speed",
                HostName = "localhost"
            };

            const int limit = 10000;
            int count = 0;
            int failureCount = 0;
            int redeliviered = 0;
            var completed = new TaskCompletionSource<bool>();

            using (var connection = connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    var consumer = new RabbitMqBasicConsumer(model, new Uri("rabbitmq://localhost/speed/input"), context =>
                        {
                            return Task.Run(() =>
                                {
                                    if (context.Redelivered)
                                        Interlocked.Increment(ref redeliviered);
                                    if (Interlocked.Increment(ref failureCount) % 50 == 0)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    var number = Interlocked.Increment(ref count);
                                    if (number == limit)
                                        completed.TrySetResult(true);
                                });
                        });


                    model.QueueDeclare("input", false, true, true, new Dictionary<string, object>());
                    model.QueuePurge("input");

                    model.ExchangeDeclare("fast", ExchangeType.Fanout, false, true, new Dictionary<string, object>());
                    model.QueueBind("input", "fast", "");

                    model.BasicQos(0, 100, false);
                    model.BasicConsume("input", false, consumer);

                    using (var sendModel = new HaModel(connection.CreateModel()))
                    {
                        byte[] body = Encoding.UTF8.GetBytes(new string('*', 8192));
                        Stopwatch timer = Stopwatch.StartNew();
                        var tasks = Enumerable.Range(0, limit)
                            .Select(x =>
                                {
                                    var properties = sendModel.CreateBasicProperties();
                                    return sendModel.BasicPublishAsync("fast", "", properties, body);
                                }).AsParallel().ToArray();

                        completed.Task.Wait();
                        timer.Stop();

                        Task.WaitAll(tasks);

                        Console.WriteLine("Elapsed time for {0} messages: {1}ms", limit, timer.ElapsedMilliseconds);
                        Console.WriteLine("Messages per second: {0}", limit * 1000L / timer.ElapsedMilliseconds);

                        Console.WriteLine("Messages: {0}", count);
                        Console.WriteLine("Redelivered: {0}", redeliviered);
                    }
                }
            }
        }
    }
}