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
namespace MassTransit.Infrastructure.Timeout
{
	using System;
	using System.Linq;
	using NHibernate;
	using NHibernate.Linq;
	using Services.Timeout;

	public class NHibernateTimeoutRepository :
		ITimeoutRepository
	{
		private volatile bool _disposed;
		private ISessionFactory _sessionFactory;

		public NHibernateTimeoutRepository(ISessionFactory sessionFactory)
		{
			_sessionFactory = sessionFactory;
		}


		public void Schedule(ScheduledTimeout timeout)
		{
			ScheduledTimeout existingTimeout;
			if (TryGetExistingTimeout(timeout, out existingTimeout))
			{
				using (var session = _sessionFactory.OpenSession())
				{
					existingTimeout.ExpiresAt = timeout.ExpiresAt;
					session.Update(existingTimeout);
					session.Flush();
				}
			}
			else
			{
				using (var session = _sessionFactory.OpenSession())
				{
					session.Save(timeout);
					session.Flush();
				}
			}
		}

		public void Remove(ScheduledTimeout timeout)
		{
			ScheduledTimeout existingTimeout;
			if (TryGetExistingTimeout(timeout, out existingTimeout))
			{
				using (var session = _sessionFactory.OpenSession())
				{
					session.Delete(existingTimeout);
					session.Flush();
				}
			}
		}

		public bool TryGetNextScheduledTimeout(out ScheduledTimeout timeout)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				timeout = session.Linq<ScheduledTimeout>()
					.OrderBy(x => x.ExpiresAt)
					.FirstOrDefault();
			}

			return timeout != null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_sessionFactory = null;
			}
			_disposed = true;
		}

		private bool TryGetExistingTimeout(ScheduledTimeout timeout, out ScheduledTimeout existingTimeout)
		{
			using (var session = _sessionFactory.OpenSession())
			{
				existingTimeout = session.Linq<ScheduledTimeout>()
					.Where(x => x.Id == timeout.Id && x.Tag == timeout.Tag)
					.FirstOrDefault();

				return existingTimeout != null;
			}
		}

		~NHibernateTimeoutRepository()
		{
			Dispose(false);
		}
	}
}