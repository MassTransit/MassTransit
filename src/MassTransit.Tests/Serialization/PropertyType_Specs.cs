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
namespace MassTransit.Tests.Serialization
{
	using NUnit.Framework;

	[TestFixture]
	public class Serializing_a_property_of_type_char :
		SerializationTest
	{
		public class PropertyOfChar
		{
			public char Value { get; set; }
		}

		public class PropertyOfNullableChar
		{
			public char? Value { get; set; }
		}

		[Test]
		public void Should_handle_a_present_value()
		{
			var obj = new PropertyOfChar {Value = 'A'};

			var result = SerializeAndReturn(obj);

			result.Value.ShouldEqual(obj.Value);
		}

		[Test]
		public void Should_handle_a_string_null()
		{
			var obj = new PropertyOfChar {Value = '\0'};

			var result = SerializeAndReturn(obj);

			result.Value.ShouldEqual(obj.Value);
		}

		[Test]
		public void Should_handle_a_present_nullable_value()
		{
			var obj = new PropertyOfNullableChar {Value = 'A'};

			var result = SerializeAndReturn(obj);

			result.Value.ShouldEqual(obj.Value);
		}

		[Test]
		public void Should_handle_a_missing_nullable_value()
		{
			var obj = new PropertyOfNullableChar();

			var result = SerializeAndReturn(obj);

			result.Value.ShouldEqual(obj.Value);
		}
	}
}