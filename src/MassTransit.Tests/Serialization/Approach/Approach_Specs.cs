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
//namespace MassTransit.Tests.Serialization.Approach
//{
//	using System;
//	using System.Collections.Generic;
//	using System.Diagnostics;
//	using System.Text;
//	using Magnum.Extensions;
//	using MassTransit.Serialization.Custom;
//	using Messages;
//	using NUnit.Framework;
//
//	[TestFixture]
//	public class Approach_Specs
//	{
//
//		[Test]
//		public void A_new_way_to_say_go()
//		{
//			var message = new Envelope
//				{
//					Name = "Chris",
//					Address = "123 American Way",
//					City = "Tulsa",
//                    State = "OK",
//					Age = 12,
//					NetWorth = 100000000,
//				};
//
//			TestSerialization(message);
//		}
//
//		[Test]
//		public void A_new_way_to_say_gxo()
//		{
//
//		MoreSerialization_Specs<TSer.OuterClass message = new MoreSerialization_Specs.OuterClass
//			{
//				Inner = new MoreSerialization_Specs.InnerClass { Name = "Chris" },
//			};
//
//
//
//			TestSerialization(message);
//		}
//
//		[Test]
//		public void A_fully_loaded_object_should_be_serialized()
//		{
//			var message = new SerializationTestMessage
//			{
//				DecimalValue = 123.45m,
//				LongValue = 098123213,
//				BoolValue = true,
//				ByteValue = 127,
//				IntValue = 123,
//				DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
//				TimeSpanValue = 30.Seconds(),
//				GuidValue = Guid.NewGuid(),
//				StringValue = "Chris's Sample Code",
//				DoubleValue = 1823.172,
//			};
//
//			TestSerialization(message);
//		}
//
//		[Test]
//		public void Fault_Messages_should_be_properly_serialized()
//		{
//			var message = new SerializationTestMessage
//			{
//				DecimalValue = 123.45m,
//				LongValue = 098123213,
//				BoolValue = true,
//				ByteValue = 127,
//				IntValue = 123,
//				DateTimeValue = new DateTime(2008, 9, 8, 7, 6, 5, 4),
//				TimeSpanValue = 30.Seconds(),
//				GuidValue = Guid.NewGuid(),
//				StringValue = "Chris's Sample Code",
//				DoubleValue = 1823.172,
//			};
//
//			var faltMessage = new Fault<SerializationTestMessage>(message, new System.Net.Mail.SmtpException("Crap!"));
//
//			TestSerialization(faltMessage);
//		}
//        
//		[Test]
//		public void A_collection_of_objects_should_be_properly_serialized()
//		{
//			MoreSerialization_Specs.ContainerClass message = new MoreSerialization_Specs.ContainerClass
//			{
//				Elements = new List<MoreSerialization_Specs.OuterClass>
//						{
//							new MoreSerialization_Specs.OuterClass
//								{
//									Inner = new MoreSerialization_Specs.InnerClass {Name = "Chris"},
//								},
//							new MoreSerialization_Specs.OuterClass
//								{
//									Inner = new MoreSerialization_Specs.InnerClass {Name = "David"},
//								},
//						}
//			};
//
//			TestSerialization(message);
//		}
//
//		[Test]
//		public void A_dictionary_of_objects_should_be_properly_serialized()
//		{
//			MoreSerialization_Specs.DictionaryContainerClass message = new MoreSerialization_Specs.DictionaryContainerClass
//			{
//				Elements = new Dictionary<string, MoreSerialization_Specs.OuterClass>
//						{
//                            {"Chris", new MoreSerialization_Specs.OuterClass{Inner = new MoreSerialization_Specs.InnerClass {Name = "Chris"}}},
//							{"David", new MoreSerialization_Specs.OuterClass{Inner = new MoreSerialization_Specs.InnerClass {Name = "David"}}},
//						}
//			};
//
//			TestSerialization(message);
//		}
//
//		private void TestSerialization<T>(T message)
//			where T : class
//		{
//			CustomXmlSerializer serializer = new CustomXmlSerializer();
//
//			byte[] data = serializer.Serialize(message);
//
//			Trace.WriteLine("Result XML:");
//			Trace.WriteLine(Encoding.UTF8.GetString(data));
//
//			serializer.Deserialize(data);
//		}
//
//
//		[Test]
//		public void A_primitive_array_of_objects_should_be_properly_serialized()
//		{
//			MoreSerialization_Specs.PrimitiveArrayClass message = new MoreSerialization_Specs.PrimitiveArrayClass
//			{
//				Values = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
//
//			};
//
//			TestSerialization(message);
//		}
//
//		[Test]
//		public void An_empty_array_of_objects_should_be_properly_serialized()
//		{
//			MoreSerialization_Specs.PrimitiveArrayClass message = new MoreSerialization_Specs.PrimitiveArrayClass
//			{
//				Values = new int[] { }
//			};
//
//			TestSerialization(message);
//		}
//
//		[Test]
//		public void An_array_of_objects_should_be_properly_serialized()
//		{
//			var message = new MoreSerialization_Specs.GenericArrayClass<MoreSerialization_Specs.InnerClass>
//			{
//				Values = new[]
//						{
//							new MoreSerialization_Specs.InnerClass { Name = "Chris" },
//							new MoreSerialization_Specs.InnerClass { Name = "David" },
//						}
//			};
//
//			TestSerialization(message);
//		}
//	}
//
//	public class Envelope
//	{
//		public string Name { get; set; }
//		public string Address { get; set; }
//		public string City { get; set; }
//		public string State { get; set; }
//		public int Age { get; set; }
//		public decimal NetWorth { get; set; }
//	}
//}