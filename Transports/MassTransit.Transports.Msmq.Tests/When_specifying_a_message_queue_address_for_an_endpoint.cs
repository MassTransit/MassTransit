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
			using (MsmqEndpoint endpoint = new MsmqEndpoint(_address))
			{
				endpoint.QueuePath
					.ShouldEqual(_expectedQueuePath);
				endpoint.Uri
					.ShouldEqual(_expectedUri);
			}
		}

		[Test]
		public void A_message_queue_uri_should_convert_to_a_queue_path()
		{
			using (MsmqEndpoint endpoint = new MsmqEndpoint(_uriAddress))
			{
				endpoint.QueuePath
					.ShouldEqual(_expectedQueuePath);
				endpoint.Uri
					.ShouldEqual(_expectedUri);
			}
		}

		[Test, Ignore("Endpoints verify the queue exists, so this will fail if the remote machine does not exist")]
		public void A_remote_server_should_keep_the_remote_server_name()
		{
			const string uriString = "msmq://remote_server/queue_name";

			using (MsmqEndpoint endpoint = new MsmqEndpoint(uriString))
			{
				endpoint.QueuePath
					.ShouldEqual(@"FormatName:DIRECT=OS:remote_server\private$\queue_name");
				endpoint.Uri
					.ShouldEqual(uriString);
			}
		}


		[Test, ExpectedException(typeof (EndpointException))]
		public void An_address_cant_contain_a_path_specifier()
		{
			const string address = "msmq://localhost/test_endpoint/error_creator";

			using (MsmqEndpoint endpoint = new MsmqEndpoint(address))
			{
			}
		}
	}

    [TestFixture]
    public class When_creating_a_MessageQueueEndpoint_from_a_Uri_string
    {
        [Test]
        public void The_result_Uri_should_match_the_constructor_string()
        {
        	using (MsmqEndpoint q = new MsmqEndpoint("msmq://localhost/mt_client"))
        	{
        		Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
        		Assert.That(q.QueuePath, Is.EqualTo(@"FormatName:DIRECT=OS:localhost\private$\mt_client"));
        	}
        }

        [Test]
        public void The_resulting_Uri_should_match_the_string()
        {
        	using (MsmqEndpoint addr = new MsmqEndpoint(@"msmq://localhost/mt_client"))
        	{
        		Assert.That(addr.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
        	}
        }
    }

    [TestFixture]
    public class When_creating_a_MessageQueueEndpoint_from_a_MessageQueue
    {
        [Test]
        public void The_queue_path_should_be_correct()
        {
        	using (MsmqEndpoint q = new MsmqEndpoint(new MessageQueue("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\mt_client")))
        	{
        		Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\mt_client"));
        	}
        }

        [Test]
        public void The_queue_path_should_be_correct_for_relative_queue_names()
        {
        	using (MsmqEndpoint q = new MsmqEndpoint(new MessageQueue(".\\private$\\mt_client")))
        	{
        		Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\mt_client"));
        	}
        }

        [Test]
        public void The_Uri_should_be_correct()
        {
        	using (MsmqEndpoint q = new MsmqEndpoint(new MessageQueue("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\mt_client")))
        	{
        		Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
        	}
        }

        [Test]
        public void The_Uri_should_be_correct_for_relative_queue_names()
        {
        	using (MsmqEndpoint q = new MsmqEndpoint(new MessageQueue(".\\private$\\mt_client")))
        	{
        		Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
        	}
        }
    }
}