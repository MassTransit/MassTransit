namespace MassTransit.Tests.Serialization
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Magnum.DateTimeExtensions;
	using MassTransit.Serialization;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Performance_Specs
	{
		[Test, Explicit]
		public void Just_how_fast_is_the_custom_xml_serializer()
		{
			var message = new SerializationTestMessage
			{
				DecimalValue = 123.45m,
				LongValue = 098123213,
				BoolValue = true,
				ByteValue = 127,
				IntValue = 123,
				DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
				TimeSpanValue = 30.Seconds(),
				GuidValue = Guid.NewGuid(),
				StringValue = "Chris's Sample Code",
				DoubleValue = 1823.172,
			};

			var serializer = new CustomXmlMessageSerializer();

			for (int i = 0; i < 10; i++)
			{
				byte[] data;
				using (MemoryStream output = new MemoryStream())
				{
					serializer.Serialize(output, message);
					data = output.ToArray();
				}
				using(MemoryStream input = new MemoryStream(data))
				{
					serializer.Deserialize(input);
				}
			}

			Stopwatch timer = Stopwatch.StartNew();

			const int iterations = 50000;

			for (int i = 0; i < iterations; i++)
			{
				using (MemoryStream output = new MemoryStream())
				{
					serializer.Serialize(output, message);
				}
			}

			timer.Stop();

			long perSecond = iterations*1000/timer.ElapsedMilliseconds;

			var msg = string.Format("Serialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
			Trace.WriteLine(msg);

			byte[] sample;
			using (MemoryStream output = new MemoryStream())
			{
				serializer.Serialize(output, message);
				sample = output.ToArray();
			}

			timer = Stopwatch.StartNew();

			for (int i = 0; i < 50000; i++)
			{
				using (MemoryStream input = new MemoryStream(sample))
				{
					serializer.Deserialize(input);
				}
			}

			timer.Stop();

			perSecond = iterations * 1000 / timer.ElapsedMilliseconds;
			
			msg = string.Format("Deserialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
			Trace.WriteLine(msg);

		}
	}
}