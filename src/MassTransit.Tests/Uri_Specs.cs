// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework;
	using Util;
	using Magnum.Extensions;

	
	public class Advanced_Uri_Specs
	{
		Uri _baseUri;

		[SetUp]
		public void There_s_an_advanced_uri()
		{
			_baseUri = new Uri("rabbitmq://testUser:topSecret@localhost:5672/mt/test_queue?a_query=23");
		}

		[Test]
		public void The_uri_extensions_are_capable_of_parsing_it()
		{
			Assert.That(
				_baseUri.AppendToPath("_error")
					.ToString(), Is.EqualTo("rabbitmq://testUser:topSecret@localhost:5672/mt/test_queue_error?a_query=23"));
		}
	}
	
	public class Simple_Uri_Specs
	{
		Uri _baseUri;

		[SetUp]
		public void There_s_a_simple_uri()
		{
			_baseUri = new Uri("rabbitmq://a:5672/");
		}

		[Test]
		public void Can_append_to_path()
		{
			Assert.That(
				_baseUri.AppendToPath("q")
					.ToString(), Is.EqualTo("rabbitmq://a:5672/q"));
		}

		[Test]
		public void Can_append_to_path2()
		{
			Assert.That(
				new Uri("rabbitmq://a:5672?354")
					.AppendToPath("q").ToString(),
				Is.EqualTo("rabbitmq://a:5672/q?354"));
		}
	}
}