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
namespace MassTransit.NHibernateIntegration.Saga
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Transactions;
	using MassTransit.Saga;
	using NHibernate;
	using NHibernate.Linq;

	public class NHibernateSagaLocator<TSaga> :
		ISagaLocator<TSaga>
		where TSaga : class, ISaga
	{
		readonly ISessionFactory _sessionFactory;

		public NHibernateSagaLocator(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}

		public IEnumerable<Guid> Find(Expression<Func<TSaga, bool>> expression)
		{
			using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
			using (ISession session = _sessionFactory.OpenSession())
			{
				IQueryable<Guid> query = session.Query<TSaga>()
					.Where(expression).Select(x => x.CorrelationId);

				foreach (Guid sagaId in query)
				{
					yield return sagaId;
				}

				scope.Complete();
			}
		}
	}
}