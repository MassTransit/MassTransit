using System;
using System.Diagnostics;
using System.Threading;
using log4net;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
	using Messages;

	[TestFixture]
	public class When_using_queues_on_a_remote_server
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (When_using_queues_on_a_remote_server));

		[Test]
		[Explicit]
		public void Replies_should_be_returned_properly()
		{
			using (QueueTestContext qtc = new QueueTestContext("dtfs.ehos.tns.ndchealth.com"))
			{
				PingMessage ping = new PingMessage();

				qtc.RemoteServiceBus.Subscribe<PingMessage>(
					delegate(IMessageContext<PingMessage> context)
						{
							context.Reply(new PongMessage());
						});

				for (int index = 0; index < 10; index++)
				{
					DateTime start = DateTime.Now;

					qtc.RemoteServiceBus.Endpoint.Send(ping);

/*
					Assert.That(asyncResult, Is.Not.Null);

					Assert.That(asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10), true), Is.True,
					            "Timeout Expired Waiting For Response");

					DateTime stop = DateTime.Now;

					_log.InfoFormat("Roundtrip time = {0}ms", (stop - start).TotalMilliseconds);

					Assert.That(asyncResult.Messages, Is.Not.Null);

					Assert.That(asyncResult.Messages, Is.Not.Empty);

					PongMessage pong = asyncResult.Messages[0] as PongMessage;

					Assert.That(pong, Is.Not.Null);
*/
				}
			}
		}
	}
}