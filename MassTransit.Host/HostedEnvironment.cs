namespace MassTransit.Host
{
    using System;
    using Castle.Facilities.FactorySupport;
    using Castle.Facilities.Startable;
    using Castle.Windsor;
    using log4net;
    using ServiceBus;
    using WindsorIntegration;

    public abstract class HostedEnvironment
    {
        private readonly string _xmlFile;
        private IWindsorContainer _container;
        private ILog _log = LogManager.GetLogger(typeof (HostedEnvironment));


        protected HostedEnvironment(string xmlFile)
        {
            _xmlFile = xmlFile;
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public abstract string ServiceName { get; }
        public abstract string DispalyName { get; }
        public abstract string Description { get; }

        //provides a way to supply username and password at config time
        public virtual string Username
        {
            get { return ""; }
        }

        public virtual string Password
        {
            get { return ""; }
        }

        protected void InitializeContainer()
        {
            _log.Debug("Initializing");

            _container = new WindsorContainer(_xmlFile);
            _container.AddFacility("masstransit", new MassTransitFacility());
            _container.AddFacility("factory", new FactorySupportFacility());
            _container.AddFacility("startable", new StartableFacility());
        }

        public virtual void Start(IStartUpTime timer)
        {
            InitializeContainer();

            _log.Debug("Starting");
            //move this into interceptor?
            foreach (IHostedService hs in _container.ResolveAll<IHostedService>())
            {
                hs.Start();
                timer.RequestMoreTime(1000);
            }
        }

        public virtual void Main()
        {
        }

        public virtual void Stop()
        {
            _log.Debug("Stopping");
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

        public event Action<HostedEnvironment> Completed;

    }
}