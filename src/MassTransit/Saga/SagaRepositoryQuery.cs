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
namespace MassTransit.Saga
{
	using System;
	using System.Linq.Expressions;

	public class SagaRepositoryQuery<TSaga, TResult>
		where TSaga : class, ISaga
	{
		readonly Func<TSaga, bool> _filter;
		readonly Expression<Func<TSaga, bool>> _filterExpression;
		readonly Func<TSaga, TResult> _projection;
		readonly Expression<Func<TSaga, TResult>> _projectionExpression;

		public SagaRepositoryQuery(Expression<Func<TSaga, bool>> filter, Expression<Func<TSaga, TResult>> projection)
		{
			_filter = filter.Compile();
			_filterExpression = filter;

			_projection = projection.Compile();
			_projectionExpression = projection;
		}

		public Func<TSaga, bool> Filter
		{
			get { return _filter; }
		}

		public Expression<Func<TSaga, bool>> FilterExpression
		{
			get { return _filterExpression; }
		}

		public Func<TSaga, TResult> Projection
		{
			get { return _projection; }
		}

		public Expression<Func<TSaga, TResult>> ProjectionExpression
		{
			get { return _projectionExpression; }
		}
	}
}