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
namespace MassTransit.Transports.RabbitMq.Tests
{
	using System;
	using Magnum.TestFramework;
	using NUnit.Framework;

	[Scenario]
	public class When_converting_a_type_to_a_message_name
	{
		[Then]
		public void Should_handle_an_interface_name()
		{
			var name = new MessageName(typeof (NameEasyToo));
			name.ToString().ShouldEqual("MassTransit.Transports.RabbitMq.Tests:NameEasyToo");
		}

		[Then]
		public void Should_handle_nested_classes()
		{
			var name = new MessageName(typeof(Nested));
			name.ToString().ShouldEqual("MassTransit.Transports.RabbitMq.Tests:When_converting_a_type_to_a_message_name-Nested");
		}

		[Then]
		public void Should_handle_regular_classes()
		{
			var name = new MessageName(typeof(NameEasy));
			name.ToString().ShouldEqual("MassTransit.Transports.RabbitMq.Tests:NameEasy");
		}

		[Then]
		public void Should_throw_an_exception_on_an_open_generic_class_name()
		{
			Assert.Throws<ArgumentException>(() => new MessageName(typeof(NameGeneric<>)));
		}

		[Then]
		public void Should_handle_a_closed_single_generic()
		{
			var name = new MessageName(typeof(NameGeneric<string>));
			name.ToString().ShouldEqual("MassTransit.Transports.RabbitMq.Tests:NameGeneric--System:String--");
		}

		[Then]
		public void Should_handle_a_closed_double_generic()
		{
			var name = new MessageName(typeof(NameDoubleGeneric<string,NameEasy>));
			name.ToString().ShouldEqual("MassTransit.Transports.RabbitMq.Tests:NameDoubleGeneric--System:String::NameEasy--");
		}

		[Then]
		public void Should_handle_a_closed_double_generic_with_a_generic()
		{
			var name = new MessageName(typeof(NameDoubleGeneric<NameGeneric<NameEasyToo>,NameEasy>));
			name.ToString().ShouldEqual("MassTransit.Transports.RabbitMq.Tests:NameDoubleGeneric--NameGeneric--NameEasyToo--::NameEasy--");
		}

		class Nested
		{ }
	}

	class NameEasy
	{
	}

	interface NameEasyToo
	{
	}

	class NameGeneric<T>
	{
	}

	class NameDoubleGeneric<T1, T2>
	{
	}
}