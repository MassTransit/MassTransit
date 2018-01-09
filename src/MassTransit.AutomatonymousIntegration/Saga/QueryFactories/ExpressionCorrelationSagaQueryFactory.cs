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
namespace Automatonymous.Saga.QueryFactories
{
    using System;
    using System.Linq.Expressions;
    using CorrelationConfigurators;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public class ExpressionCorrelationSagaQueryFactory<TInstance, TData> :
        ISagaQueryFactory<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, ConsumeContext<TData>, bool>> _correlationExpression;

        public ExpressionCorrelationSagaQueryFactory(Expression<Func<TInstance, ConsumeContext<TData>, bool>> correlationExpression)
        {
            _correlationExpression = correlationExpression;
        }

        ISagaQuery<TInstance> ISagaQueryFactory<TInstance, TData>.CreateQuery(ConsumeContext<TData> context)
        {
            Expression<Func<TInstance, bool>> filter = new EventCorrelationExpressionConverter<TInstance, TData>(context)
                .Convert(_correlationExpression);

            return new SagaQuery<TInstance>(filter);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("expression", _correlationExpression.ToString());
        }
    }
}