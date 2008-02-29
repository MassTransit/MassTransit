/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.NMS.Tests
{
    using System;
    using System.Threading;
    using Internal;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_sending_a_message_to_the_queue
    {
        [Test]
        public void The_message_should_arrive()
        {
            NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/queue_name");

            IMessageSender sender = endpoint.Sender;

            SimpleMessage msg = new SimpleMessage();
            msg.Name = "Chris";

            Envelope e = new Envelope(msg);

            sender.Send(e);
        }

        [Test]
        public void The_message_should_be_retrieved()
        {
            NmsEndpoint endpoint = new NmsEndpoint("activemq://localhost:61616/queue_name");

            IMessageReceiver receiver = endpoint.Receiver;

            ManualResetEvent received = new ManualResetEvent(false);

            EnvelopeConsumer consumer = new EnvelopeConsumer(
                delegate(IEnvelope e) { received.Set(); });

            receiver.Subscribe(consumer);

            IMessageSender sender = endpoint.Sender;

            SimpleMessage msg = new SimpleMessage();
            msg.Name = "Chris";

            Envelope env = new Envelope(msg);

            sender.Send(env);

            Assert.That(received.WaitOne(TimeSpan.FromSeconds(5), true), Is.True);

            endpoint.Dispose();
        }
    }

    public delegate void EnvelopeHandler(IEnvelope e);

    public class EnvelopeConsumer :
        IEnvelopeConsumer
    {
        private readonly EnvelopeHandler _eh;

        public EnvelopeConsumer(EnvelopeHandler eh)
        {
            _eh = eh;
        }

        public bool IsHandled(IEnvelope envelope)
        {
            return true;
        }

        public void Deliver(IEnvelope envelope)
        {
            _eh(envelope);
        }
    }

    [Serializable]
    public class SimpleMessage : IMessage
    {
        private string _name;


        public SimpleMessage()
        {
        }

        public SimpleMessage(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}