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
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using MassTransit.Tests;
    using Messages;
    using NUnit.Framework;

    [TestFixture]
    public class When_sending_directly_to_an_endpoint
    {
        [Test]
        public void The_message_should_be_added_to_the_queue()
        {
            MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");
            endpoint.Purge();

            VariableMessage message = new VariableMessage("Valid");

            endpoint.Send(message);

            endpoint.VerifyMessageInQueue<VariableMessage>();
        }
    }

    [TestFixture]
    public class When_receiving_directly_from_an_endpoint
    {
        [Test]
        public void The_message_should_be_read_from_the_queue()
        {
            MsmqEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_servicebus");
            endpoint.Purge();

            VariableMessage message = new VariableMessage("Jackson");

            endpoint.Send(message);

            object obj = endpoint.Receive(TimeSpan.FromSeconds(30));

            obj.ShouldNotBeNull();
            obj.ShouldBeSameType<VariableMessage>();

        }
    }
}