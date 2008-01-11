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
            SubscriptionCacheEntry left = new SubscriptionCacheEntry(_testEndPoint.Uri);
            SubscriptionCacheEntry right = new SubscriptionCacheEntry(_testEndPoint.Uri);

			Assert.That(left, Is.EqualTo(right));
		}
		
	}
}