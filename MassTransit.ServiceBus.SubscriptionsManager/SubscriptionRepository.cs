namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using System;
    using NHibernate;
    using NHibernate.Expression;
    using System.Collections.Generic;

    public class SubscriptionRepository : ISubscriptionRepository
    {
        private ISessionFactory _factory;


        public SubscriptionRepository(ISessionFactory factory)
        {
            _factory = factory;
        }

        public void Add(Subscription subscription)
        {
            using(ISession sess = _factory.OpenSession())
            using(ITransaction tr = sess.BeginTransaction())
            {
                ICriteria crit = sess.CreateCriteria(typeof(Subscription));

                crit.Add(Expression.Eq("Address", subscription.Address))
                    .Add(Expression.Eq("Message", subscription.Message));

                Subscription obj = crit.UniqueResult<Subscription>();
                if(obj == null)
                {
                    obj = subscription;
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

        public void Deactivate(Subscription subscription)
        {
            using(ISession sess = _factory.OpenSession())
            using(ITransaction tr = sess.BeginTransaction())
            {
                ICriteria crit = sess.CreateCriteria(typeof(Subscription));
                
                crit.Add(Expression.Eq("Address", subscription.Address))
                    .Add(Expression.Eq("Message", subscription.Message));
                
                Subscription obj = crit.UniqueResult<Subscription>();
                if(obj != null)
                {
                    obj.IsActive = false;

                    sess.Update(obj);    
                }
                
                tr.Commit();
            }
            
        }

        public List<Subscription> List()
        {
            using (ISession sess = _factory.OpenSession())
            {
                ICriteria crit = sess.CreateCriteria(typeof (Subscription));

                return new List<Subscription>(crit.List<Subscription>());
            }
        }

        public List<Subscription> List(Type message)
        {
            using (ISession sess = _factory.OpenSession())
            {
                ICriteria crit = sess.CreateCriteria(typeof(Subscription));
                crit.Add(Expression.Eq("Message", message.FullName));

                return new List<Subscription>(crit.List<Subscription>());
            }
        }
    }
}