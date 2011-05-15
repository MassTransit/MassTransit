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
namespace MassTransit.Saga
{
	using System;
	using System.Linq.Expressions;
	using Util;

	public class DataEventBinder<TSaga, TMessage> :
		EventBinder<TSaga, TMessage>
	{
		private Expression<Func<TSaga, TMessage, bool>> _expression;

		public void By(Expression<Func<TSaga, TMessage, bool>> expression)
		{
			_expression = expression;
		}

		public Expression<Func<TSaga, TMessage1, bool>> GetBindExpression<TMessage1>()
		{
			return _expression.TranslateTo<Expression<Func<TSaga, TMessage1, bool>>>();
		}
	}
}