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
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_specifying_a_message_queue_address_for_an_endpoint
	{
		[SetUp]
		public void SetUp()
		{
			_address = "msmq://localhost/mt_client";
			_uriAddress = new Uri(_address);
		}

		private string _address;
		private Uri _uriAddress;

		private readonly Uri _expectedUri = new Uri("msmq://" + Environment.MachineName + "/mt_client");
		private const string _expectedQueuePath = @"FormatName:DIRECT=OS:localhost\private$\mt_client";

		[Test]
		public void A_message_queue_address_should_convert_to_a_queue_path()
		{
			MsmqEndpointAddress address = new MsmqEndpointAddress(_uriAddress);
			address.FormatName
				.ShouldEqual(_expectedQueuePath);
			address.Uri
				.ShouldEqual(_expectedUri);
			address.IsLocal
				.ShouldBeTrue();
		}

		[Test]
		public void A_message_queue_uri_should_convert_to_a_queue_path()
		{
			MsmqEndpointAddress address = new MsmqEndpointAddress(_uriAddress);
			address.FormatName
				.ShouldEqual(_expectedQueuePath);
			address.Uri
				.ShouldEqual(_expectedUri);
			address.IsLocal
				.ShouldBeTrue();
		}

		[Test]
		public void A_remote_server_should_keep_the_remote_server_name()
		{
			Uri remoteUri = new Uri("msmq://remote_server/queue_name");
			const string remoteQueuePath = @"FormatName:DIRECT=OS:remote_server\private$\queue_name";

			MsmqEndpointAddress address = new MsmqEndpointAddress(remoteUri);
			address.FormatName
				.ShouldEqual(remoteQueuePath);
			address.Uri
				.ShouldEqual(remoteUri);
			address.IsLocal
				.ShouldBeFalse();
		}


		[Test, ExpectedException(typeof(NotSupportedException))]
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
	}

	[TestFixture]
	public class When_creating_a_MessageQueueEndpoint_from_a_Uri_string
	{
		[Test]
		public void The_result_Uri_should_match_the_constructor_string()
		{
			MsmqEndpointAddress address = new MsmqEndpointAddress(new Uri("msmq://localhost/mt_client"));

			Assert.That(address.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
			Assert.That(address.FormatName, Is.EqualTo(@"FormatName:DIRECT=OS:localhost\private$\mt_client"));
		}

		[Test]
		public void The_resulting_Uri_should_match_the_string()
		{
			MsmqEndpointAddress address = new MsmqEndpointAddress(new Uri("msmq://localhost/mt_client"));

			Assert.That(address.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
		}
	}
}