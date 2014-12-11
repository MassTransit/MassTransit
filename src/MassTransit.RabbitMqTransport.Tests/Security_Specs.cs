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
namespace MassTransit.RabbitMqTransport.Tests
{
//    using System;
//    using NUnit.Framework;
//    using TestFramework;
//
//
//    [TestFixture, Explicit]
//    public class When_connecting_to_rabbitmq_with_username_password
//    {
//        [Test]
//        public void Should_remove_username_and_password_from_source_address()
//        {
//            var inputAddress = new Uri("rabbitmq://testUser:test@localhost/mttest/test_queue");
//            var sourceAddress = new Uri("rabbitmq://localhost/mttest/test_queue");
//            var future = new Future<ConsumeContext<A>>();
//
//            using (IServiceBus bus = ServiceBusFactory.New(c =>
//                {
//                    c.ReceiveFrom(inputAddress);
//                    c.UseRabbitMq(r =>
//                        {
//                            r.ConfigureHost(sourceAddress, h =>
//                                {
//                                    h.SetUsername("testUser");
//                                    h.SetPassword("test");
//                                });
//                        });
//
//                    c.Subscribe(s => s.Handler<A>(async (context) => future.Complete(context)));
//                }))
//            {
//                bus.Publish(new A());
//
//                Assert.IsTrue(future.WaitUntilCompleted(TimeSpan.FromSeconds(8)));
//            }
//
//            Assert.AreEqual(sourceAddress.ToString(), future.Value.SourceAddress.ToString());
//            Assert.AreEqual(sourceAddress.GetLeftPart(UriPartial.Authority),
//                future.Value.DestinationAddress.GetLeftPart(UriPartial.Authority));
//        }
//
//        [Test]
//        public void Should_support_the_username_password_for_a_host()
//        {
//            var inputAddress = new Uri("rabbitmq://localhost/mttest/test_queue");
//            var future = new Future<ConsumeContext<A>>();
//
//            using (IServiceBus bus = ServiceBusFactory.New(c =>
//                {
//                    c.ReceiveFrom(inputAddress);
//                    c.UseRabbitMq(r =>
//                        {
//                            r.ConfigureHost(inputAddress, h =>
//                                {
//                                    h.SetUsername("testUser");
//                                    h.SetPassword("test");
//                                });
//                        });
//
//                    c.Subscribe(s => s.Handler<A>(async (context) => future.Complete(context)));
//                }))
//            {
//                bus.Publish(new A());
//
//                Assert.IsTrue(future.WaitUntilCompleted(TimeSpan.FromSeconds(8)));
//            }
//
//            Assert.AreEqual(inputAddress.ToString(), future.Value.SourceAddress.ToString());
//        }
//
//
//        class A
//        {
//        }
//    }
}