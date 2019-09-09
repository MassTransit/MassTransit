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
namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Linq.Expressions;
    using Automatonymous;
    using Automatonymous.CorrelationConfigurators;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class CorrelationExpression_Specs
    {
        [Test]
        public void Should_convert_a_simple_correlation_expression()
        {
            var message = new CorrelatedMessage
            {
                CorrelationId = NewId.NextGuid()
            };
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