namespace MassTransit.ServiceBus.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

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
		public void Should_Equal_Itself()
		{
			Envelope e = new Envelope();
			Assert.AreEqual(e, e);
			Assert.IsTrue(e.Equals(e));
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