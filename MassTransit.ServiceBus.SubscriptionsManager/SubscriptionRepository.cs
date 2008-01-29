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
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NHibernate;
    using NHibernate.Expression;
    using System.Collections.Generic;
    using Subscriptions;

    public class SubscriptionRepository : ISubscriptionStorage
    {
        private ISessionFactory _factory;
        private readonly IMessageQueueEndpoint _theRepositorysEndpointToIgnore;
        private object addLock = new object();

        public SubscriptionRepository(ISessionFactory factory, IMessageQueueEndpoint theRepositorysEndpointToIgnore)
        {
            _factory = factory;
            _theRepositorysEndpointToIgnore = theRepositorysEndpointToIgnore;
        }

        public void Add(string messageName, Uri endpoint)
        {
            lock (addLock)
            {
                if (!IsMessageToIgnore(messageName, endpoint))
                {
                    using (ISession sess = _factory.OpenSession())
                    using (ITransaction tr = sess.BeginTransaction())
                    {
                        ICriteria crit = sess.CreateCriteria(typeof (StoredSubscription));

                        crit.Add(Expression.Eq("Address", endpoint.ToString()))
                            .Add(Expression.Eq("Message", messageName));

                        StoredSubscription obj = crit.UniqueResult<StoredSubscription>();

                        if (obj == null)
                        {
                            obj = new StoredSubscription(endpoint.ToString(), messageName);
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
            }
        }

        public void Remove(string messageName, Uri endpoint)
        {
            using (ISession sess = _factory.OpenSession())
            using (ITransaction tr = sess.BeginTransaction())
            {
                ICriteria crit = sess.CreateCriteria(typeof(StoredSubscription));

                crit.Add(Expression.Eq("Address", endpoint.ToString()))
                    .Add(Expression.Eq("Message", messageName));

                StoredSubscription obj = crit.UniqueResult<StoredSubscription>();
                if (obj != null)
                {
                    obj.IsActive = false;

                    sess.Update(obj);
                }

                tr.Commit();
            }
        }


        public bool IsMessageToIgnore(string messageName, Uri endpoint)
        {
            return
                typeof (CacheUpdateResponse).FullName.Equals(messageName) &&
                _theRepositorysEndpointToIgnore.Uri.Equals(endpoint);
        }

        public IList<Subscription> List()
        {
            using (ISession sess = _factory.OpenSession())
            {
                ICriteria crit = sess.CreateCriteria(typeof (StoredSubscription));

                return SubscriptionMapper.MapFrom(crit.List<StoredSubscription>());
            }
        }

        public IList<Subscription> List(string messageName)
        {
            using (ISession sess = _factory.OpenSession())
            {
                ICriteria crit = sess.CreateCriteria(typeof(StoredSubscription));
                crit.Add(Expression.Eq("Message", messageName));

                return SubscriptionMapper.MapFrom(crit.List<StoredSubscription>());
            }
        }


        public event EventHandler<SubscriptionChangedEventArgs> SubscriptionChanged;


        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}