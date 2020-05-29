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
