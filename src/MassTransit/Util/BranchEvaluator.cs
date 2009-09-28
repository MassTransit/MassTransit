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
	using System.Linq.Expressions;

	public class BranchEvaluator :
		ExpressionVisitor
	{
		private readonly IEvaluationCandidateSelector _candidateSelector;

		public BranchEvaluator(IEvaluationCandidateSelector candidateSelector)
		{
			_candidateSelector = candidateSelector;
		}

		public Expression Evaluate(Expression expression)
		{
			return Visit(expression);
		}

		protected override Expression Visit(Expression exp)
		{
			if (exp == null)
				return null;

			if (_candidateSelector.ShouldExpressionBeEvaluated(exp))
			{
				return EvaluateExpression(exp);
			}

			return base.Visit(exp);
		}

		private Expression EvaluateExpression(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Constant)
			{
				return expression;
			}

			Delegate fn = Expression.Lambda(typeof (Func<>).MakeGenericType(expression.Type), expression).Compile();

			return Expression.Constant(fn.DynamicInvoke(), expression.Type);
		}
	}
}