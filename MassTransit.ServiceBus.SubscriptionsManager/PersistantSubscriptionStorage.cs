/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.SubscriptionsManager
{
	using System;
	using System.Collections.Generic;
	using log4net;
	using NHibernate;
	using NHibernate.Expression;
	using Subscriptions;

	public class PersistantSubscriptionStorage :
		ISubscriptionRepository
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
		private readonly ISessionFactory _factory;

		public PersistantSubscriptionStorage(ISessionFactory factory)
		{
			_factory = factory;
		}

		#region ISubscriptionRepository Members

		public IEnumerable<Subscription> List()
		{
			using (ISession sess = _factory.OpenSession())
			{
				ICriteria crit = sess.CreateCriteria(typeof (StoredSubscription))
					.Add(Expression.Eq("IsActive", true));

				return SubscriptionMapper.MapFrom(crit.List<StoredSubscription>());
			}
		}

		public void Dispose()
		{
			_factory.Dispose();
		}

		public void Save(Subscription subscription)
		{
			try
			{
				using (ISession sess = _factory.OpenSession())
				using (ITransaction tr = sess.BeginTransaction())
				{
					ICriteria crit = sess.CreateCriteria(typeof (StoredSubscription));

					crit.Add(Expression.Eq("EndpointUri", subscription.EndpointUri.ToString()))
						.Add(Expression.Eq("Message", subscription.MessageName));

					StoredSubscription obj = crit.UniqueResult<StoredSubscription>();

					if (obj == null)
					{
						obj = new StoredSubscription(subscription.EndpointUri.ToString(), subscription.MessageName);
						sess.Save(obj);
					}
					else
					{
						obj.IsActive = true;
						sess.Update(obj);
					}

					tr.Commit();
				}
			}
			catch (Exception ex)
			{
				_log.Error(string.Format("Error adding message {0} for address {1} to the repository", subscription.MessageName, subscription.EndpointUri), ex);
				throw;
			}
		}

		public void Remove(Subscription subscription)
		{
			using (ISession sess = _factory.OpenSession())
			using (ITransaction tr = sess.BeginTransaction())
			{
				ICriteria crit = sess.CreateCriteria(typeof (StoredSubscription));

				crit.Add(Expression.Eq("Address", subscription.EndpointUri.ToString()))
					.Add(Expression.Eq("Message", subscription.MessageName));

				StoredSubscription obj = crit.UniqueResult<StoredSubscription>();
				if (obj != null)
				{
					obj.IsActive = false;

					sess.Update(obj);
				}

				tr.Commit();
			}
		}

		#endregion
	}
}