namespace SubMgr
{
    using Castle.Core;
    using Castle.Windsor;
    using log4net;
    using MassTransit;
    using MassTransit.Services.Subscriptions;
    using MassTransit.Services.Subscriptions.Server;
    using MassTransit.WindsorIntegration;
    using Microsoft.Practices.ServiceLocation;
    using Topshelf;
    using Topshelf.Configuration;

    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        private static void Main(string[] args)
        {
            _log.Info("SubMgr Loading");
           
            var cfg = RunnerConfigurator.New(c=>
                                                 {
                                                     c.SetServiceName("SampleSubscriptionService");
                                                     c.SetServiceName("MassTransit Sample Subscription Service");
                                                     c.SetServiceName("Coordinates subscriptions between multiple systems");

                                                     c.DependencyOnMsmq();
                                                     c.RunAsLocalSystem();

                                                     c.BeforeStart(a=>
                                                                       {
                                                                            IWindsorContainer container = new DefaultMassTransitContainer("castle.xml");

                                                                            container.AddComponentLifeStyle<FollowerRepository>(LifestyleType.Singleton);


                                                                            container.AddComponent<RemoteEndpointCoordinator>();
                                                                            container.AddComponent<IHostedService, SubscriptionService>();


                                                                            container.AddComponent<ISubscriptionRepository, InMemorySubscriptionRepository>();

                                                                            var wob = new WindsorObjectBuilder(container.Kernel);
                                                                            ServiceLocator.SetLocatorProvider(()=>wob);
                                                                       });

                                                     c.ConfigureService<SubscriptionManagerLifeCycle>();
                                                 });
            Runner.Host(cfg, args);
        }
    }
}