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
    using MassTransit.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class When_specifying_a_message_queue_address_for_an_endpoint
    {
        private static string _address = "msmq://localhost/mt_client";
        private static Uri _uriAddress = new Uri("msmq://localhost/mt_client");

        [Test]
        public void A_message_queue_address_should_convert_to_a_queue_path()
        {

            MsmqEndpoint endpoint = new MsmqEndpoint(_address);

            endpoint.QueuePath
                .ShouldEqual(@"FormatName:DIRECT=OS:localhost\private$\mt_client");
            endpoint.Uri.ToString()
                .ShouldEqual("msmq://localhost/mt_client");



            MsmqEndpoint endpoint2 = new MsmqEndpoint(_uriAddress);

            endpoint2.QueuePath
                .ShouldEqual(@"FormatName:DIRECT=OS:localhost\private$\mt_client");
            endpoint2.Uri.ToString()
                .ShouldEqual("msmq://localhost/mt_client");
        }

        [Test]
        public void When_using_the_machine_name_should_convert_to_localhost()
        {
            MsmqEndpoint endpoint = new MsmqEndpoint(_address);

            endpoint.QueuePath
                .ShouldEqual(@"FormatName:DIRECT=OS:chris-0295c34e6\private$\mt_client");
            endpoint.Uri.ToString()
                .ShouldEqual(@"msmq://localhost/mt_client");



            MsmqEndpoint endpoint2 = new MsmqEndpoint(_uriAddress);

            endpoint2.QueuePath
                .ShouldEqual(@"FormatName:DIRECT=OS:chris-0295c34e6\private$\mt_client");
            endpoint2.Uri.ToString()
                .ShouldEqual("msmq://localhost/mt_client");
        }

        [Test, ExpectedException(typeof (EndpointException))]
        public void An_address_cant_contain_a_path_specifier()
        {
            string address = "msmq://localhost/test_endpoint/error_creator";

            new MsmqEndpoint(address);
        }
    }
}