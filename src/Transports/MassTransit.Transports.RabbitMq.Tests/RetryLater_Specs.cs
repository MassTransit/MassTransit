// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class RetryLaterTests
    {
        [Test]
        public void CanRetryWhenUsingHandlerWithLocalBus()
        {
            using (IServiceBus bus = ServiceBusFactory.New(sbc => sbc.ReceiveFrom("loopback://localhost/test")))
                DoTest(bus, bus);
        }

        [Test]
        public void CanRetryWhenUsingHandlerWithSingleBus()
        {
            using (IServiceBus bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.ReceiveFrom(QueueUri("handler_retry_tests"));
                    sbc.UseRabbitMq();
                }))
                DoTest(bus, bus);
        }

        [Test]
        public void CanRetryWhenUsingHandlerWithTwoBusses()
        {
            using (IServiceBus receivingBus = ServiceBusFactory.New(sbc =>
                {
                    sbc.ReceiveFrom(QueueUri("handler_retry_tests"));
                    sbc.UseRabbitMq();
                }))
            using (IServiceBus publishingBus = ServiceBusFactory.New(sbc =>
                {
                    sbc.ReceiveFrom(QueueUri("handler_retry_tests_publish"));
                    sbc.UseRabbitMq();
                }))
                DoTest(publishingBus, receivingBus);
        }

        const string RabbitMqServer = "localhost";

        static string QueueUri(string queueName)
        {
            return string.Format("rabbitmq://{0}/{1}", RabbitMqServer, queueName);
        }

        static void DoTest(IServiceBus publishingBus, IServiceBus receivingBus)
        {
            int numberOfFailures = new Random().Next(5) + 1;

            int receiveCount = 0;
            Guid testId = Guid.NewGuid();
            Trace.WriteLine(string.Format("Number of failures for message: {0}", numberOfFailures));

            using (var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                receivingBus.SubscribeContextHandler<Message>(context =>
                    {
                        try
                        {
                            Message message = context.Message;

                            // ignore messages from older tests
                            if (message.TestId != testId)
                                return;

                            Trace.Write("Processing message...", "Handler");
                            receiveCount++;

                            // retry as many times as requested by message
                            if (context.RetryCount < message.NumberOfFailures)
                            {
                                Trace.WriteLine(string.Format("Message will be retried (retry count = {0}).",
                                    context.RetryCount));
                                context.RetryLater();
                            }
                            else
                            {
                                Trace.WriteLine("Message processed.");
                                waitHandle.Set();
                            }
                        }
                        catch (Exception exc)
                        {
                            Trace.WriteLine(exc, "Handler failed");
                            throw;
                        }
                    });

                publishingBus.Publish(new Message {NumberOfFailures = numberOfFailures, TestId = testId});

                Assert.True(waitHandle.WaitOne(Debugger.IsAttached
                                                   ? TimeSpan.FromHours(1)
                                                   : TimeSpan.FromSeconds(10)));
            }

            Assert.AreEqual(numberOfFailures + 1, receiveCount);
        }

        public class Message
        {
            public int NumberOfFailures { get; set; }
            public Guid TestId { get; set; }
        }
    }
}