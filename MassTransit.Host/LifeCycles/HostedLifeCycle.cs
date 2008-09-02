namespace MassTransit.Host.LifeCycles
{
    using System;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
    using Castle.Windsor;
    using ServiceBus;
    using WindsorIntegration;

    public delegate void Work();

    public abstract class HostedLifeCycle :
        IApplicationLifeCycle
    {
        private IWindsorContainer _container;
        private readonly string _xmlFile;

        protected HostedLifeCycle(string xmlFile)
        {
            _xmlFile = xmlFile;
            _container = new WindsorContainer(_xmlFile);
            _container.AddFacility("masstransit", new MassTransitFacility());
            _container.AddFacility("factory", new FactorySupportFacility());
            _container.AddFacility("startable", new StartableFacility());
        }
        
        public void PerformWorkInAlternateThread(Work work)
        {
            work.BeginInvoke(delegate(IAsyncResult ar)
                                 {
                                     
                                 }, null);
        }

        public void Initialize()
        {   
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


            if(Completed != null)
            {
                Action<IApplicationLifeCycle> handler = Completed;
                handler(this);
            }
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        //TODO: WTF is this (and I wrote it!)
        public event Action<IApplicationLifeCycle> Completed;
    }
}