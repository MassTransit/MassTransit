// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Internals.Reflection;


    public class SagaFilterExpressionConverter<TSaga, TMessage> :
        ExpressionVisitor
    {
        readonly TMessage _message;

        public SagaFilterExpressionConverter(TMessage message)
        {
            _message = message;
        }

        public Expression<Func<TSaga, bool>> Convert(Expression<Func<TSaga, TMessage, bool>> expression)
        {
            Expression result = Visit(expression);

            return RemoveMessageParameter(result as LambdaExpression);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression != null &&
                (m.Expression.NodeType == ExpressionType.Parameter && m.Expression.Type == typeof(TMessage)))
            {
                return EvaluateMemberAccess(m);
            }

            return base.VisitMember(m);
        }

        static Expression<Func<TSaga, bool>> RemoveMessageParameter(LambdaExpression lambda)
        {
            var parameters = new[] {lambda.Parameters[0]};

            return Expression.Lambda<Func<TSaga, bool>>(lambda.Body, parameters);
        }

        Expression EvaluateMemberAccess(MemberExpression exp)
        {
            var parameter = exp.Expression as ParameterExpression;

            Delegate fn = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(TMessage), exp.Type), exp, parameter).CompileFast();

            return Expression.Constant(fn.DynamicInvoke(_message), exp.Type);
        }
    }
}
