namespace MassTransit.Dashboard.Controllers
{
    using Castle.MonoRail.Framework;
    using MassTransit.ServiceBus.Subscriptions;

    public class HomeController : Controller
    {
        private readonly ISubscriptionCache _cache;

        public HomeController(ISubscriptionCache cache)
        {
            _cache = cache;
        }

        public void View()
        {
            this.PropertyBag.Add("subscriptions", _cache.List());
        }
    }
}