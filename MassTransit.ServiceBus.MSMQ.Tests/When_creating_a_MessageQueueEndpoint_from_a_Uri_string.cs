namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using System.Messaging;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_creating_a_MessageQueueEndpoint_from_a_Uri_string
	{
		[Test]
		public void The_result_Uri_should_match_the_constructor_string()
		{
			MsmqEndpoint q = new MsmqEndpoint("msmq://localhost/test_endpoint");
			Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
			Assert.That(q.QueuePath, Is.EqualTo(@"FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\test_endpoint"));
		}

		[Test]
		public void The_resulting_Uri_should_match_the_string()
		{
			MsmqEndpoint addr = @"msmq://localhost/test_endpoint";
			Assert.That(addr.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
		}
	}

	[TestFixture]
	public class When_creating_a_MessageQueueEndpoint_from_a_MessageQueue
	{
		[Test]
		public void The_queue_path_should_be_correct()
		{
			IMsmqEndpoint q = new MsmqEndpoint(new MessageQueue("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\test_endpoint"));

			Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\test_endpoint"));
		}

		[Test]
		public void The_queue_path_should_be_correct_for_relative_queue_names()
		{
			IMsmqEndpoint q = new MsmqEndpoint(new MessageQueue(".\\private$\\test_endpoint"));

			Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\test_endpoint"));
		}

		[Test]
		public void The_Uri_should_be_correct()
		{
			IMsmqEndpoint q = new MsmqEndpoint(new MessageQueue("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\test_endpoint"));
			Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
		}

		[Test]
		public void The_Uri_should_be_correct_for_relative_queue_names()
		{
			IMsmqEndpoint q = new MsmqEndpoint(new MessageQueue(".\\private$\\test_endpoint"));
			Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/test_endpoint"));
		}
	}
}