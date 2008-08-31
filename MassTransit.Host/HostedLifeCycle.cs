namespace MassTransit.Host
{
    using System;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
    using Castle.Windsor;
    using ServiceBus;
    using WindsorIntegration;

    public abstract class HostedLifeCycle :
        IDisposable
    {
        private IWindsorContainer _container;
        private readonly string _xmlFile;

        protected HostedLifeCycle(string xmlFile)
        {
            _xmlFile = xmlFile;
        }
        
        public void Initialize()
        {   
            _container = new WindsorContainer(_xmlFile);
            _container.AddFacility("masstransit", new MassTransitFacility());
            _container.AddFacility("factory", new FactorySupportFacility());
            _container.AddFacility("startable", new StartableFacility());




            foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
            {
                hs.Start();
            }
        }

        public abstract void Start();

        public abstract void Stop();

        public void Dispose()
        {
            foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
            {
                hs.Stop();
            }

            foreach (IServiceBus bus in _container.ResolveAll<IServiceBus>())
            {
                bus.Dispose();
                _container.Release(bus);
            }

            _container.Dispose();

        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }


        //TODO: WTF is this (and I wrote it!)
        public event Action<HostedEnvironment> Completed;
    }
}