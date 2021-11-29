namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Linq.Expressions;
    using MassTransit.Saga;
    using NUnit.Framework;
    using SagaStateMachine;
    using TestFramework;


    [TestFixture]
    public class CorrelationExpression_Specs
    {
        [Test]
        public void Should_convert_a_simple_correlation_expression()
        {
            var message = new CorrelatedMessage {CorrelationId = NewId.NextGuid()};
            ConsumeContext<CorrelatedMessage> consumeContext = new TestConsumeContext<CorrelatedMessage>(message);
            var converter = new EventCorrelationExpressionConverter<TestState, CorrelatedMessage>(consumeContext);

            Expression<Func<TestState, bool>> result = converter
                .Convert((state, context) => state.CorrelationId == context.Message.CorrelationId);

            Console.WriteLine(result);
        }


        class TestState :
            SagaStateMachineInstance
        {
            public TestState(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public TestState()
            {
            }

            public Guid CorrelationId { get; set; }
        }


        class CorrelatedMessage
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
