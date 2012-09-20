// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.NHibernateIntegration.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using MassTransit.Subscriptions.Coordinator;
    using NHibernate;

    public class NHibernateSubscriptionStorage :
        SubscriptionStorage
    {
        static readonly ILog _log = Logger.Get<NHibernateSubscriptionStorage>();

        readonly ISessionFactory _sessionFactory;

        public NHibernateSubscriptionStorage(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void Dispose()
        {
        }

        public void Add(PersistentSubscription subscription)
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                PersistentSubscription existingSubscription = session.QueryOver<PersistentSubscription>()
                    .Where(x => x.BusUri == subscription.BusUri)
                    .Where(x => x.PeerId == subscription.PeerId)
                    .Where(x => x.SubscriptionId == subscription.SubscriptionId)
                    .SingleOrDefault();

                if (existingSubscription != null)
                {
                    _log.DebugFormat("Updating: {0}", existingSubscription);

                    existingSubscription.Updated = DateTime.UtcNow;
                    session.Update(existingSubscription);
                }
                else
                {
                    _log.DebugFormat("Adding: {0}", subscription);

                    session.Save(subscription);
                }

                transaction.Commit();
            }
        }

        public void Remove(PersistentSubscription subscription)
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                IList<PersistentSubscription> existingSubscription = session.QueryOver<PersistentSubscription>()
                    .Where(x => x.BusUri == subscription.BusUri)
                    .Where(x => x.PeerId == subscription.PeerId)
                    .Where(x => x.SubscriptionId == subscription.SubscriptionId)
                    .List();

                foreach (PersistentSubscription existing in existingSubscription)
                {
                    _log.DebugFormat("Removing: {0}", existing);

                    session.Delete(existing);
                }

                transaction.Commit();
            }
        }

        public IEnumerable<PersistentSubscription> Load(Uri busUri)
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                IList<PersistentSubscription> existingSubscription = session.QueryOver<PersistentSubscription>()
                    .Where(x => x.BusUri == busUri)
                    .OrderBy(x => x.PeerId).Asc
                    .List();

                transaction.Commit();

                return existingSubscription;
            }
        }
    }
}