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
    using Magnum.TestFramework;
    using NUnit.Framework;

    [TestFixture, Integration]
    public class When_specifying_a_message_queue_address_for_an_endpoint
    {
        [SetUp]
        public void SetUp()
        {
            _address = "msmq://localhost/mt_should_not_exist";
            _uriAddress = new Uri(_address);
        }

        private string _address;
        private Uri _uriAddress;

        private readonly Uri _expectedUri = new Uri("msmq://" + Environment.MachineName + "/mt_should_not_exist?tx=false&recoverable=true");
        private const string _expectedQueuePath = @"FormatName:DIRECT=OS:localhost\private$\mt_should_not_exist";

        [Test]
        public void A_message_queue_address_should_convert_to_a_queue_path()
        {
            var address = new MsmqEndpointAddress(_uriAddress);
            address.InboundFormatName
                .ShouldEqual(_expectedQueuePath);
            address.Uri
                .ShouldEqual(_expectedUri);
            address.IsLocal
                .ShouldBeTrue();
        }

        [Test]
        public void A_message_queue_address_should_not_include_query_string_parameters()
        {
            var uriAddress = new Uri("msmq://localhost/mt_should_not_exist?tx=false");

            var address = new MsmqEndpointAddress(uriAddress);

            address.InboundFormatName
                .ShouldEqual(_expectedQueuePath);
            address.IsTransactional	
                .ShouldBeFalse();
            address.Uri
                .ShouldEqual(_expectedUri);
        }

        [Test]
        public void A_message_queue_address_accept_the_tx_flag_as_a_uri_string()
        {
            var uriAddress = new Uri("msmq://localhost/mt_should_not_exist?tx=true");

            var address = new MsmqEndpointAddress(uriAddress);

            address.InboundFormatName
                .ShouldEqual(_expectedQueuePath);
            address.Uri
                .ShouldEqual(new Uri("msmq://" + Environment.MachineName + "/mt_should_not_exist?tx=true&recoverable=true"));
            address.IsTransactional
                .ShouldBeTrue();
        }

        [Test]
        public void A_message_queue_uri_should_convert_to_a_queue_path()
        {
            var address = new MsmqEndpointAddress(_uriAddress);
            address.InboundFormatName
                .ShouldEqual(_expectedQueuePath);
            address.Uri
                .ShouldEqual(_expectedUri);
            address.IsLocal
                .ShouldBeTrue();
        }

        [Test]
        public void A_remote_server_should_keep_the_remote_server_name()
        {
            var remoteUri = new Uri("msmq://remote_server/queue_name");
            const string remoteQueuePath = @"FormatName:DIRECT=OS:remote_server\private$\queue_name";

            var address = new MsmqEndpointAddress(remoteUri);
            address.InboundFormatName
                .ShouldEqual(remoteQueuePath);
            address.Uri
                .ShouldEqual(remoteUri);
            address.IsLocal
                .ShouldBeFalse();
        }


        [Test, ExpectedException(typeof (NotSupportedException))]
        public void An_address_cant_contain_a_path_specifier()
        {
            const string address = "msmq://localhost/test_endpoint/error_creator";

            new MsmqEndpointAddress(new Uri(address));
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void Public_queues_are_not_supported()
        {
            const string address = "msmq://localhost/public/bob";

            new MsmqEndpointAddress(new Uri(address));
        }

        [Test]
        public void Should_property_convert_multicast_to_local()
        {
            Uri multicast = new Uri("msmq-pgm://205.10.13.15/my_queue");

            Uri local = multicast.GetInboundUri();

            local.ShouldEqual(new Uri("msmq://" + Environment.MachineName.ToLowerInvariant() + "/my_queue"));
        }

        [Test]
        public void Local_queues_should_be_non_transactional_by_default()
        {
            var address = new MsmqEndpointAddress(_uriAddress);

            address.InboundFormatName
                .ShouldEqual(_expectedQueuePath);
            address.Uri
                .ShouldEqual(_expectedUri);
            address.IsLocal
                .ShouldBeTrue();
            address.IsTransactional
                .ShouldBeFalse();
        }

        [Test]
        public void Remote_queue_should_be_non_transactional_when_identified()
        {
            var remoteUri = new Uri("msmq://remote_server/queue_name?tx=false");
            const string remoteQueuePath = @"FormatName:DIRECT=OS:remote_server\private$\queue_name";

            var address = new MsmqEndpointAddress(remoteUri);

            address.InboundFormatName
                .ShouldEqual(remoteQueuePath);
            address.Uri
                .ShouldEqual(remoteUri);
            address.IsLocal
                .ShouldBeFalse();
            address.IsTransactional
                .ShouldBeFalse();
        }

        [Test]
        public void Local_queue_should_be_transactional_when_identified()
        {
            var address = new MsmqEndpointAddress(new Uri(_address + "?tx=true"));

            address.InboundFormatName
                .ShouldEqual(_expectedQueuePath);
            address.Uri
                .ShouldEqual(new Uri(_expectedUri.ToString().Replace("?tx=false", "?tx=true")));
            address.IsLocal
                .ShouldBeTrue();
            address.IsTransactional
                .ShouldBeTrue();
        }
    }

    [TestFixture, Integration]
    public class When_creating_a_MessageQueueEndpoint_from_a_Uri_string
    {
        [Test]
        public void The_result_Uri_should_match_the_constructor_string()
        {
            var address = new MsmqEndpointAddress(new Uri("msmq://localhost/mt_should_not_exist"));

            Assert.That(address.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_should_not_exist?tx=false&recoverable=true"));
            Assert.That(address.InboundFormatName, Is.EqualTo(@"FormatName:DIRECT=OS:localhost\private$\mt_should_not_exist"));
        }

        [Test]
        public void The_resulting_Uri_should_match_the_string()
        {
            var address = new MsmqEndpointAddress(new Uri("msmq://localhost/mt_should_not_exist"));

            Assert.That(address.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_should_not_exist?tx=false&recoverable=true"));
        }
    }
}