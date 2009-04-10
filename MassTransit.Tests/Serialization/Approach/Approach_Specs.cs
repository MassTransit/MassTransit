// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Tests.Serialization.Approach
{
	using System;
	using System.Diagnostics;
	using System.Text;
	using Magnum.DateTimeExtensions;
	using Messages;
	using NUnit.Framework;

	[TestFixture]
	public class Approach_Specs
	{
		[Test]
		public void A_new_way_to_say_go()
		{

			var message = new Envelope
				{
					Name = "Chris",
					Address = "123 American Way",
					City = "Tulsa",
                    State = "OK",
					Age = 12,
					NetWorth = 100000000,
				};

			CustomXmlSerializer serializer = new CustomXmlSerializer();

			byte[] data = serializer.Serialize(message);

			Trace.WriteLine("Result XML:");
			Trace.WriteLine(Encoding.UTF8.GetString(data));
		}

		[Test]
		public void A_new_way_to_say_gxo()
		{

			MoreSerialization_Specs.OuterClass message = new MoreSerialization_Specs.OuterClass
			{
				Inner = new MoreSerialization_Specs.InnerClass { Name = "Chris" },
			};



			CustomXmlSerializer serializer = new CustomXmlSerializer();

			byte[] data = serializer.Serialize(message);

			Trace.WriteLine("Result XML:");
			Trace.WriteLine(Encoding.UTF8.GetString(data));
		}

		[Test]
		public void A_collection_of_objects_should_be_properly_serialized()
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

			CustomXmlSerializer serializer = new CustomXmlSerializer();

			byte[] data = serializer.Serialize(message);

			Trace.WriteLine("Result XML:");
			Trace.WriteLine(Encoding.UTF8.GetString(data));
		}



	}

	public class Envelope
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public int Age { get; set; }
		public decimal NetWorth { get; set; }
	}
}