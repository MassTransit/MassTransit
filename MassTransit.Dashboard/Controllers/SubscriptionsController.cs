namespace MassTransit.Dashboard.Controllers
{
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
            this.PropertyBag.Add("subscriptions", _cache.List());
        }
    }
}