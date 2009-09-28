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
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public class EvaluationCandidateSelector :
		ExpressionVisitor, IEvaluationCandidateSelector
	{
		private HashSet<Expression> _candidates = new HashSet<Expression>();
		private bool _evaluationAllowed;

		public bool ShouldExpressionBeEvaluated(Expression expression)
		{
			return _candidates.Contains(expression);
		}

		public void IdentifyCandidates(Expression expression)
		{
			_candidates.Clear();

			Visit(expression);
		}

		protected override Expression Visit(Expression exp)
		{
			if (exp == null)
				return null;

			bool previousEvaluationAllowed = _evaluationAllowed;

			_evaluationAllowed = true;

			base.Visit(exp);

			if (_evaluationAllowed)
			{
				if (CanEvaluateExpressionLocally(exp))
				{
					_candidates.Add(exp);
				}
				else
				{
					_evaluationAllowed = false;
				}
			}

			if (!previousEvaluationAllowed)
				_evaluationAllowed = false;

			return exp;
		}

		private bool CanEvaluateExpressionLocally(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Parameter)
				return false;

			if (typeof (IQueryable).IsAssignableFrom(expression.Type))
				return false;

			return true;
		}
	}
}