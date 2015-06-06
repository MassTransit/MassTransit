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
namespace MassTransit.Tests.Saga
{
    using System;
    using System.Linq.Expressions;
    using MassTransit.Saga;
    using Messages;
    using NUnit.Framework;
    using Shouldly;


    public class When_using_correlated_messages_to_start_sagas
    {
        CorrelationExpressionToSagaIdVisitor<SimpleSaga, InitiateSimpleSaga> _builder;

        [SetUp]
        public void Using_correlated_messages_to_start_sagas()
        {
            _builder = new CorrelationExpressionToSagaIdVisitor<SimpleSaga, InitiateSimpleSaga>();
        }

        [Test]
        public void Should_return_null_if_the_expression_does_not_use_a_correlation_id()
        {
            Expression<Func<InitiateSimpleSaga, Guid>> expression = _builder.Build((saga, message) => 1 == 3);

            expression.ShouldBe(null);
        }

        [Test]
        public void Should_return_an_expression_that_gets_the_correlation_value_if_matched()
        {
            Expression<Func<InitiateSimpleSaga, Guid>> exp = _builder.Build((saga, message) => saga.CorrelationId == message.CorrelationId);

            exp.ShouldNotBe(null);

            Func<InitiateSimpleSaga, Guid> call = exp.Compile();

            var m = new InitiateSimpleSaga(Guid.NewGuid());

            Guid value = call(m);

            value.ShouldBe(m.CorrelationId);
        }
    }
}