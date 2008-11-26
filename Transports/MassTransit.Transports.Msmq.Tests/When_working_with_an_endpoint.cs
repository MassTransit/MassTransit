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
    using System.Messaging;
    using Exceptions;
    using NUnit.Framework;

    [TestFixture]
    public class When_working_with_an_endpoint
    {
        [Test, ExpectedException(typeof(EndpointException))]
        public void An_exception_should_be_thrown_for_a_non_existant_queue()
        {
            MsmqEndpoint q = new MsmqEndpoint(new Uri("msmq://localhost/this_queue_does_not_exist"));

            q.Open(QueueAccessMode.ReceiveAndAdmin).GetAllMessages();
        }
    }
}