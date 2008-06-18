namespace SubscriptionServiceHost
{
    using System.IO;
    using Castle.Core;
    using log4net;
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.SubscriptionStorage;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.xml"));
            _log.Info("SubMgr Loading");

            HostedEnvironment env = new SubscriptionManagerEnvironment("pubsub.castle.xml");

            env.Container.AddComponentLifeStyle("followerrepository", typeof(FollowerRepository), LifestyleType.Singleton);
            env.Container.AddComponentLifeStyle("addsubscriptionhandler", typeof(AddSubscriptionHandler), LifestyleType.Transient);
            env.Container.AddComponentLifeStyle("removesubscriptionhandler", typeof(RemoveSubscriptionHandler), LifestyleType.Transient);
            env.Container.AddComponentLifeStyle("cacheupdaterequesthandler", typeof(CacheUpdateRequestHandler), LifestyleType.Transient);
            env.Container.AddComponentLifeStyle("cancelupdaterequesthandler", typeof(CancelUpdatesHandler), LifestyleType.Transient);

            env.Container.AddComponent<IHostedService, SubscriptionService>();

            env.Container.AddComponent<ISubscriptionRepository, NHibernateSubscriptionStorage>();

            Runner.Run(env, args);
        }
    }
}