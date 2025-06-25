namespace MassTransit.Tests.Saga.Locator;

using System;
using MassTransit.Configuration;
using MassTransit.Saga;
using NUnit.Framework;
using TestFramework;


[TestFixture]
public class Using_a_property_saga_query_expression
{
    [Test]
    public void Should_allow_extraction_of_property_name_and_value()
    {
        var propertySelector = new SagaQueryPropertySelector<Message, string>(x => x.Message.Name);

        var factory = new PropertyExpressionSagaQueryFactory<Instance, Message, string>(x => x.MemberName, propertySelector);

        var consumeContext = new TestConsumeContext<Message>(new Message { Name = "Joe" });

        Assert.That(factory.TryCreateQuery(consumeContext, out ISagaQuery<Instance> query));

        Assert.That(query.TryGetPropertyValue(out var value));

        Assert.That(value, Is.EqualTo("Joe"));
    }

    [Test]
    public void Should_allow_extraction_of_property_name_and_value_by_type()
    {
        var propertySelector = new SagaQueryPropertySelector<Message, string>(x => x.Message.Name);

        var factory = new PropertyExpressionSagaQueryFactory<Instance, Message, string>(x => x.MemberName, propertySelector);

        var consumeContext = new TestConsumeContext<Message>(new Message { Name = "Joe" });

        Assert.That(factory.TryCreateQuery(consumeContext, out ISagaQuery<Instance> query));

        Assert.That(query.TryGetPropertyValue<Instance, string>(out var value));

        Assert.That(value, Is.EqualTo("Joe"));
    }


    class Instance :
        SagaStateMachineInstance
    {
        public string MemberName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    class Message
    {
        public string Name { get; set; }
    }
}
