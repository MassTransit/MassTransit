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
namespace MassTransit.Tests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class What_does_the_uri_do :
		Specification
	{
		[Test]
		public void An_address_in_an_invalid_format_should_throw_an_exception()
		{
			string address = @".\private$\bogus";

			Assert.Throws<UriFormatException>(() => { new Uri(address); });
		}

		[Test]
		public void The_address_should_be_entered_in_a_URI_style_format()
		{
			string address = "local://localhost/test_endpoint";

			var endpointUri = new Uri(address);

			Assert.That(endpointUri.Scheme, Is.EqualTo("local"));

			Assert.That(endpointUri.Host, Is.EqualTo("localhost"));

			Assert.That(endpointUri.AbsolutePath, Is.EqualTo("/test_endpoint"));
		}
	}
}