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
    using Exceptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_specifying_a_message_queue_address_for_an_endpoint
    {
        [Test]
        public void A_message_queue_address_should_convert_to_a_queue_path()
        {
            string address = "msmq://localhost/mt_client";

            MsmqEndpoint endpoint = new MsmqEndpoint(address);

            Assert.That(endpoint.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + @"\private$\mt_client"));
            Assert.That(endpoint.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));


            MsmqEndpoint endpoint2 = new MsmqEndpoint(new Uri(address));

            Assert.That(endpoint2.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + @"\private$\mt_client"));
            Assert.That(endpoint2.Uri.ToString(), Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
        }

        [Test, ExpectedException(typeof (EndpointException))]
        public void An_address_cant_contain_a_path_specifier()
        {
            string address = "msmq://localhost/test_endpoint/error_creator";

            new MsmqEndpoint(address);
        }
    }
}