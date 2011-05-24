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
	using Magnum.TestFramework;
	using NUnit.Framework;
	using Util;
	using Magnum.Extensions;

	[Scenario]
	public class Uri_Specs
	{
		Uri _baseUri;

		[When]
		public void There_s_an_advanced_uri()
		{
			_baseUri = new Uri("rabbitmq://testUser:topSecret@localhost:5672/mt/test_queue");
		}

		[Then]
		public void The_uri_extensions_are_capable_of_parsing_it()
		{
			Assert.That(
				_baseUri.AppendToPath("_error")
					.ToString(), Is.EqualTo("rabbitmq://testUser:topSecret@localhost:5672/mt/test_queue_error"));
		}
	}
}