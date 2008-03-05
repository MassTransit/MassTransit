namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class MsmqMessageId_Specs
	{
		[Test]
		public void A_newly_created_MessageId_should_be_empty()
		{
			IMessageId id = new MsmqMessageId();

			Assert.That(id, Is.EqualTo(MsmqMessageId.Empty));
		}

		[Test]
		public void A_zero_string_should_be_an_empty_MessageId()
		{
			string value = @"{00000000-0000-0000-0000-000000000000}\0";

			MsmqMessageId id = value;

			Assert.That(id, Is.EqualTo(MsmqMessageId.Empty));
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void How_to_handle_bad_ids_with_no_sequence()
		{
			Envelope e = new Envelope();
			e.Id = new MsmqMessageId("5DF5FF14-DA6B-495f-8292-6FAD060FA13A");
		}

		[Test]
		public void Should_not_equal_another_with_same_guid_diff_sequence()
		{
			Envelope e = new Envelope();
			e.Id = new MsmqMessageId("5DF5FF14-DA6B-495f-8292-6FAD060FA13A\\1");

			Envelope n = new Envelope();
			n.Id = new MsmqMessageId("5DF5FF14-DA6B-495f-8292-6FAD060FA13A\\2");

			Assert.AreNotEqual(e, n);
			Assert.IsFalse(e.Equals(n));
		}

		[Test]
		public void The_CorrelationId_Should_Be_Set()
		{
			MsmqMessageId id = Guid.NewGuid() + "\\27";

			Envelope e = new Envelope();

			e.CorrelationId = id;

			Assert.That(e.CorrelationId, Is.EqualTo(id));
		}

		[Test]
		public void The_Id_Should_Be_Set()
		{
			MsmqMessageId id = Guid.NewGuid() + "\\27";

			Envelope e = new Envelope();

			e.Id = id;

			Assert.That(e.Id, Is.EqualTo(id));
		}

		[Test]
		public void When_comparing_two_MessageId_objects_they_should_be_equal()
		{
			MsmqMessageId firstId = Guid.NewGuid() + @"\12345";
			MsmqMessageId secondId = firstId;

			Assert.That(firstId, Is.EqualTo(secondId));
		}
	}
}