using log4net.Config;

[assembly : XmlConfigurator(ConfigFile = "log4net.xml", Watch = true)]

namespace OpenAllNight.PubSub
{
    using Castle.Core;
    using log4net;
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.ServerHandlers;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("SubMgr Loading");

            HostedEnvironment env = new SubscriptionManagerEnvironment("pubsub.castle.xml");

            env.Container.AddComponentLifeStyle("followerrepository", typeof (FollowerRepository), LifestyleType.Singleton);
            env.Container.AddComponentLifeStyle("addsubscriptionhandler", typeof (AddSubscriptionHandler), LifestyleType.Transient);
            env.Container.AddComponentLifeStyle("removesubscriptionhandler", typeof (RemoveSubscriptionHandler), LifestyleType.Transient);
            env.Container.AddComponentLifeStyle("cacheupdaterequesthandler", typeof (CacheUpdateRequestHandler), LifestyleType.Transient);
            env.Container.AddComponentLifeStyle("cancelupdaterequesthandler", typeof (CancelUpdatesHandler), LifestyleType.Transient);

            env.Container.AddComponent<IHostedService, SubscriptionService>();

            env.Container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();

            Runner.Run(env, args);
        }
    }
}