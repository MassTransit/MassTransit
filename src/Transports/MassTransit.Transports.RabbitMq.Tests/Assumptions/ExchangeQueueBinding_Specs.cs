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
namespace MassTransit.Transports.RabbitMq.Tests.Assumptions
{
    using System.Collections.Generic;
    using System.Text;
    using Magnum.TestFramework;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Framing;


    [Scenario]
    public abstract class Given_a_rabbitmq_server
    {
        public IConnection Connection { get; set; }
        public IModel Model { get; private set; }

        public ConnectionFactory Factory { get; set; }

        [Given]
        public void A_rabbitmq_server()
        {
            Factory = TestFactory.ConnectionFactory();

            Connection = Factory.CreateConnection();
            Model = Connection.CreateModel();
        }

        [Finally]
        public void Finally()
        {
            Model.Dispose();
            Model = null;
            Connection.Dispose();
            Connection = null;
        }
    }


    [Scenario]
    public class When_an_exchange_is_bound_to_a_queue :
        Given_a_rabbitmq_server
    {
        string _queueName;

        [When]
        public void An_exchange_is_bound_to_a_queue()
        {
            Model.ExchangeDeclare("TypeA", ExchangeType.Fanout, true, true, null);
            _queueName = Model.QueueDeclare("TypeA", true, true, true, null);

            Model.QueueBind(_queueName, "TypeA", "");
            Model.QueuePurge(_queueName);

            byte[] message = Encoding.UTF8.GetBytes("HELLO, WORLD.");

            IBasicProperties properties = new BasicProperties();
            properties.Type = "System.string";

            Model.BasicPublish("TypeA", "", properties, message);
        }

        [Then]
        public void Should_receive_messages_sent_to_the_exchange()
        {
            BasicGetResult x = Model.BasicGet(_queueName, true);
            x.Exchange.ShouldEqual("TypeA");
        }
    }


    [Scenario]
    public class When_an_inheritance_chain_is_built_using_exchanges :
        Given_a_rabbitmq_server
    {
        string _queueName;

        [When]
        public void An_exchange_is_bound_to_a_queue()
        {
            Model.ExchangeDeclare("TheClass", ExchangeType.Fanout, true, true, null);

            Model.ExchangeDeclare("InterfaceA", ExchangeType.Fanout, true, true, null);
            Model.ExchangeBind("InterfaceA", "TheClass", "");

            Model.ExchangeDeclare("InterfaceB", ExchangeType.Fanout, true, true, null);
            Model.ExchangeBind("InterfaceB", "InterfaceA", "");

            _queueName = Model.QueueDeclare("Consumer", true, true, true, null);

            Model.QueueBind(_queueName, "InterfaceB", "");
            Model.QueuePurge(_queueName);

            byte[] message = Encoding.UTF8.GetBytes("HELLO, WORLD.");

            IBasicProperties properties = new BasicProperties();
            properties.Type = "System.string";

            Model.BasicPublish("TheClass", "", properties, message);
        }

        [Then]
        public void Should_receive_messages_sent_to_the_exchange()
        {
            BasicGetResult x = Model.BasicGet(_queueName, true);
            x.Exchange.ShouldEqual("TheClass");
        }
    }


    [Scenario]
    public class When_an_exchange_is_bound_to_an_exchange :
        Given_a_rabbitmq_server
    {
        [When]
        public void An_exchange_is_bound_to_a_queue()
        {
            Model.ExchangeDeclare("TheSource", ExchangeType.Fanout, true, true, null);
            Model.ExchangeDeclare("TheDestination", ExchangeType.Fanout, true, true, null);
            Model.QueueDeclare("TheQueue", true, true, true, null);
            Model.QueueBind("TheQueue", "TheDestination", "");

            Model.ExchangeBind("TheDestination", "TheSource", "");
        }

        [Then]
        public void Should_be_able_to_unbind_the_exchanges()
        {
            Model.ExchangeUnbind("TheDestination", "TheSource", "");
        }
    }


    [Scenario]
    public class When_an_exchange_is_bound_to_a_high_available_queue :
        Given_a_rabbitmq_server
    {
        string _queueName;

        [When]
        public void An_exchange_is_bound_to_a_highly_available_queue()
        {
            var args = new Dictionary<string, object>();
            args.Add("x-ha-policy", "all");
            Model.ExchangeDeclare("TypeA", ExchangeType.Fanout, true, true, null);
            _queueName = Model.QueueDeclare("TypeA", true, true, true, args);

            Model.QueueBind(_queueName, "TypeA", "");
            Model.QueuePurge(_queueName);

            byte[] message = Encoding.UTF8.GetBytes("HELLO, WORLD.");

            IBasicProperties properties = new BasicProperties();
            properties.Type = "System.string";

            Model.BasicPublish("TypeA", "", properties, message);
        }

        [Then]
        public void Should_receive_messages_sent_to_the_exchange()
        {
            BasicGetResult x = Model.BasicGet(_queueName, true);
            x.Exchange.ShouldEqual("TypeA");
        }
    }
}