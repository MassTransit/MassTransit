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
namespace MassTransit.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using log4net;
    using NHibernate;
    using NHibernate.Criterion;
    using ServiceBus.Subscriptions;

    public class PersistantSubscriptionRepository :
        ISubscriptionRepository
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
        private readonly ISessionFactory _factory;

        public PersistantSubscriptionRepository(ISessionFactory factory)
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