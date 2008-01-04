namespace MassTransit.ServiceBus.SubscriptionsManager
{
    using NHibernate;
    using NHibernate.Expression;
    using System.Collections.Generic;

    public class SubscriptionRepository
    {
        private ISessionFactory _factory;

        public void Add(Subscription subscription)
        {
            using(ISession sess = _factory.OpenSession())
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


            }
        }

        public void Deactivate(Subscription subscription)
        {
            using(ISession sess = _factory.OpenSession())
            {
                ICriteria crit = sess.CreateCriteria(typeof(Subscription));
                
                crit.Add(Expression.Eq("Address", subscription.Address))
                    .Add(Expression.Eq("Message", subscription.Message));
                
                Subscription obj = crit.UniqueResult<Subscription>();
                obj.IsActive = false;

                sess.Update(obj);
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
    }
}