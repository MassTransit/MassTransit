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
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;


    public class PropertyExpressionSagaQueryFactory<TInstance, TData, TProperty> :
        ISagaQueryFactory<TInstance, TData>
        where TInstance : class, SagaStateMachineInstance
        where TData : class
    {
        readonly Expression<Func<TInstance, TProperty>> _propertyExpression;
        readonly Func<ConsumeContext<TData>, TProperty> _selector;

        public PropertyExpressionSagaQueryFactory(Expression<Func<TInstance, TProperty>> propertyExpression, Func<ConsumeContext<TData>, TProperty> selector)
        {
            _propertyExpression = propertyExpression;
            _selector = selector;
        }

        ISagaQuery<TInstance> ISagaQueryFactory<TInstance, TData>.CreateQuery(ConsumeContext<TData> context)
        {
            TProperty propertyValue = _selector(context);

            Expression<Func<TInstance, bool>> filterExpression = CreateExpression(propertyValue);

            return new SagaQuery<TInstance>(filterExpression);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("property", _propertyExpression.ToString());
        }

        Expression<Func<TInstance, bool>> CreateExpression(TProperty propertyValue)
        {
            ConstantExpression valueExpression = Expression.Constant(propertyValue, typeof(TProperty));
            BinaryExpression binaryExpression = Expression.Equal(_propertyExpression.Body, valueExpression);
            Expression<Func<TInstance, bool>> lambdaExpression = Expression.Lambda<Func<TInstance, bool>>(binaryExpression, _propertyExpression.Parameters);

            return lambdaExpression;
        }
    }
}