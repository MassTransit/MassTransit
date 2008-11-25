namespace MassTransit.MSMQ.Tests
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
			MsmqEndpoint q = new MsmqEndpoint("msmq://localhost/mt_client");
			Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
			Assert.That(q.QueuePath, Is.EqualTo(@"FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\mt_client"));
		}

		[Test]
		public void The_resulting_Uri_should_match_the_string()
		{
            MsmqEndpoint addr = @"msmq://localhost/mt_client";
            Assert.That(addr.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
		}
	}

	[TestFixture]
	public class When_creating_a_MessageQueueEndpoint_from_a_MessageQueue
	{
		[Test]
		public void The_queue_path_should_be_correct()
		{
            MsmqEndpoint q = new MsmqEndpoint(new MessageQueue("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\mt_client"));

            Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\mt_client"));
		}

		[Test]
		public void The_queue_path_should_be_correct_for_relative_queue_names()
		{
            MsmqEndpoint q = new MsmqEndpoint(new MessageQueue(".\\private$\\mt_client"));

            Assert.That(q.QueuePath, Is.EqualTo("FormatName:DIRECT=OS:" + Environment.MachineName.ToLowerInvariant() + "\\private$\\mt_client"));
		}

		[Test]
		public void The_Uri_should_be_correct()
		{
            MsmqEndpoint q = new MsmqEndpoint(new MessageQueue("FormatName:DIRECT=OS:" + Environment.MachineName + @"\private$\mt_client"));
            Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
		}

		[Test]
		public void The_Uri_should_be_correct_for_relative_queue_names()
		{
            MsmqEndpoint q = new MsmqEndpoint(new MessageQueue(".\\private$\\mt_client"));
            Assert.That(q.Uri.AbsoluteUri, Is.EqualTo("msmq://" + Environment.MachineName.ToLowerInvariant() + "/mt_client"));
		}
	}
}