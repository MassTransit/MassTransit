namespace MassTransit.Host
{
    using System;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
    using Castle.Windsor;
    using ServiceBus;
    using WindsorIntegration;

    public abstract class HostedEnvironment
    {
        private readonly IWindsorContainer _container;


        protected HostedEnvironment(string xmlFile) 
        {
            _container = new WindsorContainer(xmlFile);
            _container.AddFacility("masstransit", new MassTransitFacility());
            _container.AddFacility("factory", new FactorySupportFacility());
            _container.AddFacility("startable", new StartableFacility());
        }
        protected HostedEnvironment()
        {
            _container = new WindsorContainer();
        }


        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public virtual void Start()
        {
            //move this into interceptor?
            foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
            {
                hs.Start();
            }
        }

        public virtual void Main()
        {
        }

        public virtual void Stop()
        {
            //move this into interceptor?
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

        public abstract string ServiceName {get;}
        public abstract string DispalyName {get;}
        public abstract string Description {get;}

        //provides a way to supply username and password at config time
        public virtual string Username
        {
            get
            {
                return "";
            }
        }
        public virtual string Password
        {
            get
            {
                return "";
            }
        }

        public event Action<HostedEnvironment> Completed;

        internal delegate void Handler();
    }
}