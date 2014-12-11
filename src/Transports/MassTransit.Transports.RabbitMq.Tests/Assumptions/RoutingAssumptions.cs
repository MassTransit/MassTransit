namespace MassTransit.RabbitMqTransport.Tests.Assumptions
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class RoutingAssumptions :
        GivenAChannel
    {
        [Test]
        public void Run()
        {
            WithChannel(model =>
                {
                    model.QueueDeclare("testqueue", true, false, false, null);
                    model.ExchangeDeclare("testtopic", "topic", true, true, null);
                    model.QueueBind("testqueue", "testtopic", "#.Ping.#");
                    model.QueuePurge("testqueue");


                    model.BasicPublish("testtopic", "Message.Ping.Pong", null, new byte[] {1, 2, 3});
                    model.BasicPublish("testtopic", "Ping.Pong", null, new byte[] {1, 2, 3});
                    model.BasicPublish("testtopic", "Ping", null, new byte[] {1, 2, 3});



                    var x = model.BasicGet("testqueue", true);
                    x.MessageCount.ShouldBeEqualTo<uint>(2);

                });
        }

        [Test]
        public void WhatDoesOneQueueOneExchange25BindingsLookLike()
        {
            var queues = "mgmtqueue0,mgmtqueue1,mgmtqueue2,mgmtqueue3,mgmtqueue4";
            var sampleNames = "Ping,Pong,LoginEvent,LoginSucceded,LoginFailed,LoanInformation,CreateNewLoan,SendMessage,NewSubscription," +
                              "SubscriptionRemoved,SubscriptionClientAdded,Heartbeat,NewWorker,ConsolidatedCustomerInformation";

            WithChannel(model =>
                {
                    model.ExchangeDeclare("mgmtexchange", "topic", true, true, null);
                    queues.Split(',').Each(q =>
                        {
                            model.QueueDeclare(q, true, false, false, null);
                            sampleNames.Split(',').Each(name =>
                                {
                                    model.QueueBind(q, "mgmtexchange", "#.{0}.#".FormatWith(name));
                                    model.QueuePurge(q);

                                });

                        });
                });
        }
    }
}