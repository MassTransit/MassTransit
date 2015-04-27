// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Transports;


    public class When_converting_a_type_to_a_message_name
    {
        IMessageNameFormatter _formatter;

        public When_converting_a_type_to_a_message_name()
        {
            _formatter = new RabbitMqMessageNameFormatter();
        }

        [Test]
        public void Should_handle_an_interface_name()
        {
            MessageName name = _formatter.GetMessageName(typeof(NameEasyToo));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameEasyToo");
        }

        [Test]
        public void Should_handle_nested_classes()
        {
            MessageName name = _formatter.GetMessageName(typeof(Nested));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:When_converting_a_type_to_a_message_name-Nested");
        }

        [Test]
        public void Should_handle_regular_classes()
        {
            MessageName name = _formatter.GetMessageName(typeof(NameEasy));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameEasy");
        }

        [Test]
        public void Should_throw_an_exception_on_an_open_generic_class_name()
        {
            Assert.Throws<ArgumentException>(() => _formatter.GetMessageName(typeof(NameGeneric<>)));
        }

        [Test]
        public void Should_handle_a_closed_single_generic()
        {
            MessageName name = _formatter.GetMessageName(typeof(NameGeneric<string>));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameGeneric--System:String--");
        }

        [Test]
        public void Should_handle_a_closed_double_generic()
        {
            MessageName name = _formatter.GetMessageName(typeof(NameDoubleGeneric<string, NameEasy>));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameDoubleGeneric--System:String::NameEasy--");
        }

        [Test]
        public void Should_handle_a_closed_double_generic_with_a_generic()
        {
            MessageName name = _formatter.GetMessageName(typeof(NameDoubleGeneric<NameGeneric<NameEasyToo>, NameEasy>));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameDoubleGeneric--NameGeneric--NameEasyToo--::NameEasy--");
        }


        class Nested
        {
        }
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