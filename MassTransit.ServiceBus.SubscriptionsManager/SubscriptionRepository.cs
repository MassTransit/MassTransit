namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using NHibernate;
    using NHibernate.Expression;
    using System.Collections.Generic;
    using Subscriptions;

    public class SubscriptionRepository : ISubscriptionStorage
    {
        private ISessionFactory _factory;


        public SubscriptionRepository(ISessionFactory factory)
        {
            _factory = factory;
        }

        public void Add(string messageName, Uri endpoint)
        {
            using (ISession sess = _factory.OpenSession())
            using (ITransaction tr = sess.BeginTransaction())
            {
                ICriteria crit = sess.CreateCriteria(typeof(StoredSubscription));

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