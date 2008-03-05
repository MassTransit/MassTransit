namespace MassTransit.ServiceBus.DefermentService.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class DefermentServiceTDD
	{
		private TimeSpan oneSecond = new TimeSpan(0, 0, 1);

		private MockRepository _mocks = new MockRepository();

		[Test]
		public void Doodle()
		{
			IMessage msg = null;
			IDefermentService d = new DefermentService();
			//when deferment completes we send back to the queue as if it had just come in.
			int defermentClaimTicket = d.Defer(msg, oneSecond);
		}

		[Test]
		public void Doodle2()
		{
			ServiceBus bus = new ServiceBus(_mocks.CreateMock<IEndpoint>());
			bus.SubscriptionStorage = _mocks.CreateMock<ISubscriptionStorage>();
			IDefermentService d = new DefermentService();

			int defermentClaimTicket = 0;

			//this should get called twice effectively
			//once for the first deferring call
			//once for post-deferment
			bus.Subscribe<DelayMessage>(delegate(IMessageContext<DelayMessage> cxt)
			                            	{
			                            		if (cxt.Message.Deferred == false)
			                            		{
			                            			defermentClaimTicket = d.Defer(cxt.Message, oneSecond);
			                            			cxt.Message.Deferred = true;
			                            		}
			                            		else
			                            		{
			                            			Assert.That(defermentClaimTicket, Is.EqualTo(1));
			                            		}
			                            	});


			//my question is since this is async how can I make it wait?
			//got any mad MultiThread ideas?
		}
	}

	[Serializable]
	public class DelayMessage :
		IMessage
	{
		public bool Deferred = false;
	}
}