namespace MassTransit.Dashboard.Controllers
{
    using System.Collections.Generic;
    using Castle.MonoRail.Framework;
    using MassTransit.ServiceBus.Subscriptions;

    [Layout("default")]
    public class SubscriptionsController : Controller
    {
        private readonly ISubscriptionCache _cache;

        public SubscriptionsController(ISubscriptionCache cache)
        {
            _cache = cache;
        }

        public void View()
        {
            List <Subscription> subs = new List<Subscription>(_cache.List());
            subs.Sort(delegate(Subscription left, Subscription right)
                          {
                              return left.EndpointUri.ToString().CompareTo(right.EndpointUri.ToString());
                          });
            this.PropertyBag.Add("subscriptions", subs);
        }
    }
}