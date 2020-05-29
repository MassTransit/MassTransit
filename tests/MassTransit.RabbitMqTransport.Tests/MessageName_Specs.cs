namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Transports;


    public class When_converting_a_type_to_a_message_name
    {
        readonly IMessageNameFormatter _formatter;

        public When_converting_a_type_to_a_message_name()
        {
            _formatter = new RabbitMqMessageNameFormatter();
        }

        [Test]
        public void Should_handle_an_interface_name()
        {
            var name = _formatter.GetMessageName(typeof(NameEasyToo));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameEasyToo");
        }

        [Test]
        public void Should_handle_nested_classes()
        {
            var name = _formatter.GetMessageName(typeof(Nested));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:When_converting_a_type_to_a_message_name-Nested");
        }

        [Test]
        public void Should_handle_regular_classes()
        {
            var name = _formatter.GetMessageName(typeof(NameEasy));
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
            var name = _formatter.GetMessageName(typeof(NameGeneric<string>));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameGeneric--System:String--");
        }

        [Test]
        public void Should_handle_a_closed_double_generic()
        {
            var name = _formatter.GetMessageName(typeof(NameDoubleGeneric<string, NameEasy>));
            name.ToString().ShouldBe("MassTransit.RabbitMqTransport.Tests:NameDoubleGeneric--System:String::NameEasy--");
        }

        [Test]
        public void Should_handle_a_closed_double_generic_with_a_generic()
        {
            var name = _formatter.GetMessageName(typeof(NameDoubleGeneric<NameGeneric<NameEasyToo>, NameEasy>));
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
