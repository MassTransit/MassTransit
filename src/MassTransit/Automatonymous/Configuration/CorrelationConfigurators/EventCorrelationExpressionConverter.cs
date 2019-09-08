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
namespace Automatonymous.CorrelationConfigurators
{
    using System;
    using System.Linq.Expressions;
    using MassTransit;
    using MassTransit.Internals.Reflection;


    public class EventCorrelationExpressionConverter<TInstance, TMessage> :
        ExpressionVisitor
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;

        public EventCorrelationExpressionConverter(ConsumeContext<TMessage> context)
        {
            _context = context;
        }

        public Expression<Func<TInstance, bool>> Convert(Expression<Func<TInstance, ConsumeContext<TMessage>, bool>> expression)
        {
            Expression result = Visit(expression);

            return RemoveMessageParameter(result as LambdaExpression);
        }

        static Expression<Func<TInstance, bool>> RemoveMessageParameter(LambdaExpression lambda)
        {
            var parameters = new[] {lambda.Parameters[0]};

            return Expression.Lambda<Func<TInstance, bool>>(lambda.Body, parameters);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression == null)
                return base.VisitMember(m);

            if (m.Expression.NodeType == ExpressionType.Parameter && m.Expression.Type == typeof(ConsumeContext<TMessage>))
                return EvaluateConsumeContextAccess(m);

            return base.VisitMember(m);
        }

        Expression EvaluateConsumeContextAccess(MemberExpression exp)
        {
            var parameter = exp.Expression as ParameterExpression;

            Delegate fn = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(ConsumeContext<TMessage>), exp.Type), exp, parameter)
                .CompileFast();

            return Expression.Constant(fn.DynamicInvoke(_context), exp.Type);
        }
    }
}
