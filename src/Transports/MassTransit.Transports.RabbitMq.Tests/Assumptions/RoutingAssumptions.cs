namespace MassTransit.Transports.RabbitMq.Tests.Assumptions
{
    using Magnum.Extensions;
    using Magnum.TypeScanning;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Magnum.TestFramework;

    [TestFixture]
    public class RoutingAssumptions
    {
        [Test]
        public void Run()
        {
            var cf = new ConnectionFactory();
            cf.UserName = "guest";
            cf.Password = "guest";
            cf.Port = 5672;
            cf.VirtualHost = "/";
            cf.HostName = "localhost";

            using (var conn = cf.CreateConnection())
            {
                using (var model = conn.CreateModel())
                {
                    model.QueueDeclare("testqueue", true, false, false, null);
                    model.ExchangeDeclare("testtopic", "topic", true, true, null);
                    model.QueueBind("testqueue", "testtopic","#.Ping.#");
                    model.QueuePurge("testqueue");

                    
                    model.BasicPublish("testtopic", "Message.Ping.Pong", null, new byte[]{1,2,3});
                    model.BasicPublish("testtopic", "Ping.Pong", null, new byte[]{1,2,3});
                    model.BasicPublish("testtopic", "Ping", null, new byte[]{1,2,3});

                    

                    var x = model.BasicGet("testqueue", true);
                    ((int)x.MessageCount).ShouldBeEqualTo(2);

                }

            }
        }

        [Test]
        public void WhatDoesOneQueueOneExchange25BindingsLookLike()
        {
            var cf = new ConnectionFactory();
            cf.UserName = "guest";
            cf.Password = "guest";
            cf.Port = 5672;
            cf.VirtualHost = "/";
            cf.HostName = "localhost";

            var queues = "mgmtqueue0,mgmtqueue1,mgmtqueue2,mgmtqueue3,mgmtqueue4";
            var sampleNames = "Ping,Pong,LoginEvent,LoginSucceded,LoginFailed,LoanInformation,CreateNewLoan,SendMessage,NewSubscription,"+
                "SubscriptionRemoved,SubscriptionClientAdded,Heartbeat,NewWorker,ConsolidatedCustomerInformation";

            using (var conn = cf.CreateConnection())
            {
                using (var model = conn.CreateModel())
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

                }

            }
        }
    }
}