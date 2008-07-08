namespace MassTransit.ServiceBus.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_a_message_should_expire_after_a_period_of_time
	{
		[Test]
		public void The_message_should_use_an_expiration_attribute()
		{
			MyMessage message = new MyMessage();

			object[] attributes = message.GetType().GetCustomAttributes(typeof (ExpiresIn), false);

			Assert.That(attributes.Length, Is.GreaterThan(0));

			foreach (ExpiresIn expiresIn in attributes)
			{
				Assert.That(expiresIn.TimeSpan, Is.EqualTo(TimeSpan.FromMinutes(5)));
			}
		}

		[ExpiresIn("00:05:00")]
		internal class MyMessage
		{
		}
	}

	[TestFixture]
	public class When_a_message_needs_to_be_reliable
	{
		[Test]
		public void The_reliable_attribute_should_be_specified()
		{
			MyMessage message = new MyMessage();

			object[] attributes = message.GetType().GetCustomAttributes(typeof(Reliable), false);

			Assert.That(attributes.Length, Is.GreaterThan(0));

			foreach (Reliable reliable in attributes)
			{
				Assert.That(reliable.Enabled, Is.True);
			}
			
		}

		[Test]
		public void The_reliable_attribute_should_be_able_to_be_false()
		{
			MyOtherMessage message = new MyOtherMessage();

			object[] attributes = message.GetType().GetCustomAttributes(typeof(Reliable), false);

			Assert.That(attributes.Length, Is.GreaterThan(0));

			foreach (Reliable reliable in attributes)
			{
				Assert.That(reliable.Enabled, Is.False);
			}
			
		}

		[Reliable]
		internal class MyMessage
		{
			
		}

		[Reliable(false)]
		internal class MyOtherMessage
		{
			
		}
	}
}