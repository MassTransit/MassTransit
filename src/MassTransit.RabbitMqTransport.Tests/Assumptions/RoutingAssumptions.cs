// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests.Assumptions
{
    using NUnit.Framework;
    using RabbitMQ.Client;
    using Shouldly;


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


                BasicGetResult x = model.BasicGet("testqueue", true);
                x.MessageCount.ShouldBe<uint>(2);
            });
        }

        [Test]
        public void WhatDoesOneQueueOneExchange25BindingsLookLike()
        {
            string queues = "mgmtqueue0,mgmtqueue1,mgmtqueue2,mgmtqueue3,mgmtqueue4";
            string sampleNames = "Ping,Pong,LoginEvent,LoginSucceded,LoginFailed,LoanInformation,CreateNewLoan,SendMessage,NewSubscription," +
                "SubscriptionRemoved,SubscriptionClientAdded,Heartbeat,NewWorker,ConsolidatedCustomerInformation";

            WithChannel(model =>
            {
                model.ExchangeDeclare("mgmtexchange", "topic", true, true, null);
                foreach (string q in queues.Split(','))
                {
                    model.QueueDeclare(q, true, false, false, null);
                    foreach (string name in sampleNames.Split(','))
                    {
                        model.QueueBind(q, "mgmtexchange", string.Format("#.{0}.#", name));
                        model.QueuePurge(q);
                    }
                }
            });
        }
    }
}