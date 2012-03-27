// Copyright 2007-2010 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Tests.Serialization
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using Context;
	using Magnum.Extensions;
	using MassTransit.Serialization;
	using Messages;
	using NUnit.Framework;


	public abstract class Performance_Specs<TSerializer> where TSerializer : IMessageSerializer, new()
	{
		[Test, Explicit]
		public void Just_how_fast_are_you()
		{
			Trace.WriteLine("Serializer: " + typeof(TSerializer).Name);
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


			var serializer = new TSerializer();

            //warm it up
			for (int i = 0; i < 10; i++)
			{
				byte[] data;
				using (var output = new MemoryStream())
				{
					serializer.Serialize(output, message.ToSendContext());
					data = output.ToArray();
				}
				using (var input = new MemoryStream(data))
				{
					serializer.Deserialize(ReceiveContext.FromBodyStream(input));
				}
			}

			Stopwatch timer = Stopwatch.StartNew();

			const int iterations = 50000;

			for (int i = 0; i < iterations; i++)
			{
				using (var output = new MemoryStream())
				{
					serializer.Serialize(output, message.ToSendContext());
				}
			}

			timer.Stop();

			long perSecond = iterations*1000/timer.ElapsedMilliseconds;

			var msg = string.Format("Serialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
			Trace.WriteLine(msg);

			byte[] sample;
			using (var output = new MemoryStream())
			{
				serializer.Serialize(output, message.ToSendContext());
				sample = output.ToArray();
			}

			timer = Stopwatch.StartNew();

			for (int i = 0; i < 50000; i++)
			{
				using (var input = new MemoryStream(sample))
				{
					serializer.Deserialize(ReceiveContext.FromBodyStream(input));
				}
			}

			timer.Stop();

			perSecond = iterations*1000/timer.ElapsedMilliseconds;

			msg = string.Format("Deserialize: {0}ms, Rate: {1} m/s", timer.ElapsedMilliseconds, perSecond);
			Trace.WriteLine(msg);
		}
	}

    [TestFixture]
    public class WhenUsingVersionOneXmlInPerfTest:
        Performance_Specs<VersionOneXmlMessageSerializer>
    {
    }

    [TestFixture]
    public class WhenUsingCustomXmlInPerfTest:
        Performance_Specs<XmlMessageSerializer>
    {
    }

    [TestFixture]
    public class WhenUsingBinaryInPerfTest :
        Performance_Specs<BinaryMessageSerializer>
    {
    }

    [TestFixture]
    public class WhenUsingJsonInPerfTest :
        Performance_Specs<JsonMessageSerializer>
    {
        
    }

	[TestFixture]
    public class WhenUsingBsonInPerfTest :
        Performance_Specs<BsonMessageSerializer>
    {
        
    }
}