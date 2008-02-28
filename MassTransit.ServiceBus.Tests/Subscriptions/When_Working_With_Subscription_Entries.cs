namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_Working_With_Subscription_Entries
	{
		#region Setup/Teardown

		[SetUp]
		public virtual void Before_Each_Test_In_The_Fixture()
		{
			_serviceBusEndPoint = _mocks.CreateMock<IEndpoint>();

			SetupResult.For(_serviceBusEndPoint.Uri).Return(new Uri(_serviceBusQueueName));

			_mocks.ReplayAll();
		}

		#endregion

		protected MockRepository _mocks = new MockRepository();

		protected IServiceBus _serviceBus;
		protected IEndpoint _serviceBusEndPoint;
		protected string _serviceBusQueueName = @"msmq://localhost/test_servicebus";

		[Test]
		public void Comparing_Two_Entries_Should_Return_True()
		{
			SubscriptionCacheEntry left = new SubscriptionCacheEntry(_serviceBusEndPoint.Uri);
			SubscriptionCacheEntry right = new SubscriptionCacheEntry(_serviceBusEndPoint.Uri);

			Assert.That(left, Is.EqualTo(right));
		}
	}
}