namespace MassTransit.Patterns.Tests
{
	using System;
	using System.IO;
	using Batching;
	using MassTransit.ServiceBus.Formatters;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
	using ServiceBus;

	[TestFixture]
	public class When_serializing_batch_messages
	{
		[Test]
		public void The_message_should_survive_serialization()
		{
			MockRepository mocks = new MockRepository();

			IFormattedBody mockBody = mocks.CreateMock<IFormattedBody>();

			StringBatchMessage bm = new StringBatchMessage(Guid.NewGuid(), 1, "Hello");

			XmlBodyFormatter formatter = new XmlBodyFormatter();

			MemoryStream memoryStream = new MemoryStream();

			using (mocks.Record())
			{
				Expect.Call(mockBody.BodyStream).Return(memoryStream).Repeat.Any();
			}

			using (mocks.Playback())
			{
				formatter.Serialize(mockBody, bm);

				memoryStream.Position = 0;

				IMessage[] result = formatter.Deserialize(mockBody);

				Assert.That(result, Is.Not.Null);
				Assert.That(result.Length, Is.EqualTo(1));
			}
		}
	}

	public class StringBatchMessage : BatchMessage<string, Guid>
	{
		public StringBatchMessage(Guid batchId, int batchLength, string body) :
			base(batchId, batchLength, body)
		{
		}

		private StringBatchMessage()
		{
		}
	}
}