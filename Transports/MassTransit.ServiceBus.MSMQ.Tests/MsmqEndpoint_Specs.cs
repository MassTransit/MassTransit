// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.ServiceBus.MSMQ.Tests
{
    using System;
    using System.Messaging;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_sending_directly_to_an_endpoint
    {
        [Test]
        public void The_message_should_be_added_to_the_queue()
        {
            MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

            using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
            {
                queue.Purge();

                CustomMessage message = new CustomMessage("Valid");

                endpoint.Send(message);

                Message msg = queue.Receive(TimeSpan.FromSeconds(3));

                object obj = new BinaryFormatter().Deserialize(msg.BodyStream);

                Assert.That(obj, Is.Not.Null);
                if (obj != null)
                {
                    Assert.That(obj, Is.TypeOf(typeof (CustomMessage)));
                }
            }
        }
    }

    [TestFixture]
    public class When_receiving_directly_from_an_endpoint
    {
        [Test]
        public void The_message_should_be_read_from_the_queue()
        {
            MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");

            using (MessageQueue queue = endpoint.Open(QueueAccessMode.ReceiveAndAdmin))
            {
                queue.Purge();

                CustomMessage message = new CustomMessage("Jackson");

                endpoint.Send(message);

                object obj = endpoint.Receive(TimeSpan.FromSeconds(30));

                Assert.That(obj, Is.Not.Null);
                if (obj != null)
                {
                    Assert.That(obj, Is.TypeOf(typeof (CustomMessage)));

                    CustomMessage response = (CustomMessage) obj;

                    Assert.That(response.Name, Is.EqualTo("Jackson"));
                }
            }
        }
    }

    [Serializable]
    public class CustomMessage
    {
        private string _name;

        public CustomMessage()
        {
        }

        public CustomMessage(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    [Serializable]
    public class WrongMessage
    {
    }
}