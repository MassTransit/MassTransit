// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Util
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;

	public class MemberAccessEvaluator<T> :
		ExpressionVisitor
	{
		private T _message;

		public MemberAccessEvaluator(T message)
		{
			_message = message;
		}

		public Expression Evaluate(Expression expression)
		{
			Expression result = Visit(expression);
			if (result.NodeType == ExpressionType.Lambda)
				return RemoveMessageParameterFromLambda(result as LambdaExpression);

			return result;
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			if (m.Expression.NodeType == ExpressionType.Parameter && m.Expression.Type == typeof (T))
			{
				return EvaluateMemberAccess(m);
			}

			return base.VisitMemberAccess(m);
		}

		private Expression RemoveMessageParameterFromLambda(LambdaExpression lambda)
		{
			if (lambda.Parameters.Where(x => x.Type == typeof(T)).Count() == 0)
				return lambda;

			var parameters = lambda.Parameters.Where(x => x.Type != typeof (T));

			Type lambdaType = RemoveMessageTypeFromLambda(lambda.Type);

			return Expression.Lambda(lambdaType, lambda.Body, parameters);
		}

		private Type RemoveMessageTypeFromLambda(Type type)
		{
			Type outerType = type.GetGenericTypeDefinition();
			var arguments = type.GetGenericArguments().Where(x => x != typeof (T));

			if(outerType == typeof(Func<,,>))
			{
				return typeof (Func<,>).MakeGenericType(arguments.ToArray());
			}

			throw new NotSupportedException();
		}

		private Expression EvaluateMemberAccess(MemberExpression exp)
		{
			var parameter = exp.Expression as ParameterExpression;

			Delegate fn = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), exp.Type), exp, new[] {parameter }).Compile();

			return Expression.Constant(fn.DynamicInvoke(_message), exp.Type);
		}
	}
}