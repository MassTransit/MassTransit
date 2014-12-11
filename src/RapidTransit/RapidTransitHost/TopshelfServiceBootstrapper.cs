namespace RapidTransit
{
    using System;
    using Autofac;
    using Topshelf.Logging;
    using Topshelf.Runtime;


    public abstract class TopshelfServiceBootstrapper<T> :
        IDisposable
        where T : TopshelfServiceBootstrapper<T>
    {
        readonly IContainer _container;
        readonly LogWriter _log = HostLogger.Get<T>();

        protected TopshelfServiceBootstrapper(HostSettings hostSettings)
        {
            _log.InfoFormat("Configuring Service Container");

            var builder = new ContainerBuilder();

            builder.RegisterInstance(hostSettings);

            ConfigureContainer(builder);

            builder.RegisterType<TopshelfHostService>()
                   .SingleInstance();

            _container = builder.Build();
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        public TopshelfHostService GetService()
        {
            return _container.Resolve<TopshelfHostService>();
        }

        protected abstract void ConfigureContainer(ContainerBuilder builder);
    }
}