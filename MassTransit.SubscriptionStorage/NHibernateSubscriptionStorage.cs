namespace MassTransit.SubscriptionStorage
{
	using System;
	using System.Collections.Generic;
	using log4net;
	using MassTransit.ServiceBus.Subscriptions;
	using NHibernate;
	using NHibernate.Expression;

	public class NHibernateSubscriptionStorage :
		ISubscriptionRepository
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
		private readonly ISessionFactory _factory;

		public NHibernateSubscriptionStorage(ISessionFactory factory)
		{
			_factory = factory;
		}

		#region ISubscriptionRepository Members

		public void Save(Subscription subscription)
		{
			try
			{
				using (ISession session = _factory.OpenSession())
				using (ITransaction transaction = session.BeginTransaction())
				{
					ICriteria criteria = session.CreateCriteria(typeof (PersistentSubscription));

					criteria.Add(Expression.Eq("Address", subscription.EndpointUri.ToString()))
						.Add(Expression.Eq("MessageName", subscription.MessageName));

					PersistentSubscription obj = criteria.UniqueResult<PersistentSubscription>();
					if (obj == null)
					{
						obj = new PersistentSubscription(subscription);
						session.Save(obj);
					}
					else
					{
						obj.IsActive = true;
						session.Update(obj);
					}

					transaction.Commit();
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
			try
			{
				using (ISession session = _factory.OpenSession())
				using (ITransaction transaction = session.BeginTransaction())
				{
					ICriteria criteria = session.CreateCriteria(typeof (PersistentSubscription));

					criteria.Add(Expression.Eq("Address", subscription.EndpointUri.ToString()))
						.Add(Expression.Eq("MessageName", subscription.MessageName));

					PersistentSubscription obj = criteria.UniqueResult<PersistentSubscription>();
					if (obj != null)
					{
						obj.IsActive = false;
						session.Update(obj);
					}

					transaction.Commit();
				}
			}
			catch (Exception ex)
			{
				_log.Error(string.Format("Error removing message {0} for address {1} from the repository", subscription.MessageName, subscription.EndpointUri), ex);
				throw;
			}
		}

		public IEnumerable<Subscription> List()
		{
			using (ISession session = _factory.OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof (PersistentSubscription))
					.Add(Expression.Eq("IsActive", true));

				List<Subscription> result = new List<Subscription>();

				IList<PersistentSubscription> activeSubscriptions = criteria.List<PersistentSubscription>();
				foreach (PersistentSubscription subscription in activeSubscriptions)
				{
					result.Add(subscription);
				}

				return result;
			}
		}

		public void Dispose()
		{
			_factory.Dispose();
		}

		#endregion
	}
}