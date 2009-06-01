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
	using Magnum.Data;
	using NHibernate;
	using NHibernate.Linq;
	using Services.Timeout;

	public class NHibernateTimeoutRepository :
		RepositoryBase<ScheduledTimeout>,
		ITimeoutRepository
	{
		private readonly object _queryLock = new object();
		private volatile bool _disposed;
		private ISession _session;

		public NHibernateTimeoutRepository(ISessionFactory sessionFactory)
		{
			_session = sessionFactory.OpenSession();
		}

		protected override IQueryable<ScheduledTimeout> RepositoryQuery
		{
			get
			{
				lock (_queryLock)
				{
					return _session.Linq<ScheduledTimeout>();
				}
			}
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Save(ScheduledTimeout item)
		{
			_session.Save(item);
		}

		public override void Update(ScheduledTimeout item)
		{
			_session.Update(item);
		}

		public override void Delete(ScheduledTimeout item)
		{
			_session.Delete(item);
		}

		public void Schedule(ScheduledTimeout timeout)
		{
			ScheduledTimeout existingTimeout;
			if (TryGetExistingTimeout(timeout, out existingTimeout))
			{
				existingTimeout.ExpiresAt = timeout.ExpiresAt;
				_session.Save(existingTimeout);
			}
			else
			{
				_session.Save(timeout);
			}

			_session.Flush();
		}

		public void Remove(ScheduledTimeout timeout)
		{
			ScheduledTimeout existingTimeout;
			if (TryGetExistingTimeout(timeout, out existingTimeout))
			{
				_session.Delete(existingTimeout);
				_session.Flush();
			}
		}

		public void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_session.Dispose();
				_session = null;
			}
			_disposed = true;
		}

		private bool TryGetExistingTimeout(ScheduledTimeout timeout, out ScheduledTimeout existingTimeout)
		{
			existingTimeout = _session.Linq<ScheduledTimeout>()
				.Where(x => x.Id == timeout.Id && x.Tag == timeout.Tag)
				.FirstOrDefault();

			return existingTimeout != null;
		}

		~NHibernateTimeoutRepository()
		{
			Dispose(false);
		}
	}
}