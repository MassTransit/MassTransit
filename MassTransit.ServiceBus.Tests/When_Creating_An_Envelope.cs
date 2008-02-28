namespace MassTransit.ServiceBus.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using Util;

	[TestFixture]
	public class When_Creating_An_Envelope
	{
		[Test]
		public void A_Message_Should_Be_Stored_In_The_Envelope()
		{
			MockRepository mocks = new MockRepository();

			IMessage message = mocks.Stub<IMessage>();

			IEnvelope e = new Envelope(message);

			Assert.That(e.Messages.Length, Is.EqualTo(1));
		}

		[Test]
		public void A_Return_Address_Should_Be_Stored_With_The_Envelope()
		{
			MockRepository mocks = new MockRepository();

			string endpointUriString = "msmq://localhost/test_queue";

			IEndpoint returnEndpoint = mocks.CreateMock<IEndpoint>();
			using (mocks.Record())
			{
				SetupResult.For(returnEndpoint.Uri).Return(new Uri(endpointUriString));
			}

			using (mocks.Playback())
			{
				IEnvelope e = new Envelope(returnEndpoint);

				Assert.That(e.ReturnEndpoint.Uri.AbsoluteUri, Is.EqualTo(endpointUriString));
			}
		}

		[Test]
		public void An_Array_Of_Messages_Should_Be_Stored_In_The_Envelope()
		{
			MockRepository mocks = new MockRepository();

			IMessage[] messages = new IMessage[2];
			messages[0] = mocks.Stub<IMessage>();
			messages[1] = mocks.Stub<IMessage>();

			IEnvelope e = new Envelope(messages);

			Assert.That(e.Messages.Length, Is.EqualTo(2));
		}

		[Test]
		public void An_Param_Array_Of_Messages_Should_Be_Stored_In_The_Envelope()
		{
			MockRepository mocks = new MockRepository();

			IMessage message = mocks.Stub<IMessage>();
			IMessage message1 = mocks.Stub<IMessage>();

			IEnvelope e = new Envelope(message, message1);

			Assert.That(e.Messages.Length, Is.EqualTo(2));
		}

		[Test]
		[ExpectedException(typeof (Exception))]
		public void How_to_handle_bad_ids_with_no_sequence()
		{
			Envelope e = new Envelope();
			e.Id = new MessageId("5DF5FF14-DA6B-495f-8292-6FAD060FA13A");
		}

		[Test]
		public void Should_Equal_Itself()
		{
			Envelope e = new Envelope();
			Assert.AreEqual(e, e);
			Assert.IsTrue(e.Equals(e));
		}

		[Test]
		public void Should_not_equal_another_with_same_guid_diff_sequence()
		{
			Envelope e = new Envelope();
			e.Id = new MessageId("5DF5FF14-DA6B-495f-8292-6FAD060FA13A\\1");

			Envelope n = new Envelope();
			n.Id = new MessageId("5DF5FF14-DA6B-495f-8292-6FAD060FA13A\\2");

			Assert.AreNotEqual(e, n);
			Assert.IsFalse(e.Equals(n));
		}

		[Test]
		public void The_ArrivedTime_Should_Be_Set()
		{
			DateTime time = DateTime.Now;

			Envelope e = new Envelope();

			e.ArrivedTime = time;

			Assert.That(e.ArrivedTime, Is.EqualTo(time));
		}

		[Test]
		public void The_CorrelationId_Should_Be_Set()
		{
			MessageId id = Guid.NewGuid() + "\\27";

			Envelope e = new Envelope();

			e.CorrelationId = id;

			Assert.That(e.CorrelationId, Is.EqualTo(id));
		}

		[Test]
		public void The_Id_Should_Be_Set()
		{
			MessageId id = Guid.NewGuid() + "\\27";

			Envelope e = new Envelope();

			e.Id = id;

			Assert.That(e.Id, Is.EqualTo(id));
		}

		[Test]
		public void The_Label_Should_Be_Set()
		{
			string id = Guid.NewGuid().ToString();

			Envelope e = new Envelope();

			e.Label = id;

			Assert.That(e.Label, Is.EqualTo(id));
		}

		[Test]
		public void The_SentTime_Should_Be_Set()
		{
			DateTime time = DateTime.Now;

			Envelope e = new Envelope();

			e.SentTime = time;

			Assert.That(e.SentTime, Is.EqualTo(time));
		}

		[Test]
		public void The_TimeToBeReceived_Should_Be_Set()
		{
			TimeSpan time = TimeSpan.FromMinutes(30);

			Envelope e = new Envelope();

			e.TimeToBeReceived = time;

			Assert.That(e.TimeToBeReceived, Is.EqualTo(time));
		}
	}
}