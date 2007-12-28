using MassTransit.ServiceBus.Subscriptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests.Subscriptions
{
	[TestFixture]
	public class When_Working_With_Subscription_Entries :
		ServiceBusSetupFixture
	{
		[Test]
		public void Comparing_Two_Entries_Should_Return_True()
		{
			MessageQueueEndpoint leftEndpoint = _testEndPoint.Address;
			MessageQueueEndpoint rightEndpoint = _testEndPoint.Address;

			SubscriptionCacheEntry left = new SubscriptionCacheEntry(leftEndpoint);
			SubscriptionCacheEntry right = new SubscriptionCacheEntry(rightEndpoint);

			Assert.That(left, Is.EqualTo(right));
		}
		
	}
}