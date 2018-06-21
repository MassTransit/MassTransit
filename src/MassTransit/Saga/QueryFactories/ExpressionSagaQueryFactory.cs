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
namespace MassTransit.Saga.QueryFactories
{
    using System;
    using System.Linq.Expressions;
    using GreenPipes;


    /// <summary>
    /// Creates a saga query using the specified filter expression
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class ExpressionSagaQueryFactory<TSaga, TMessage> :
        ISagaQueryFactory<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly Expression<Func<TSaga, TMessage, bool>> _filterExpression;

        public ExpressionSagaQueryFactory(Expression<Func<TSaga, TMessage, bool>> filterExpression)
        {
            _filterExpression = filterExpression;
        }

        ISagaQuery<TSaga> ISagaQueryFactory<TSaga, TMessage>.CreateQuery(ConsumeContext<TMessage> context)
        {
            Expression<Func<TSaga, bool>> expression = new SagaFilterExpressionConverter<TSaga, TMessage>(context.Message)
                .Convert(_filterExpression);

            return new SagaQuery<TSaga>(expression);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("expression", _filterExpression.ToString());
        }
    }
}