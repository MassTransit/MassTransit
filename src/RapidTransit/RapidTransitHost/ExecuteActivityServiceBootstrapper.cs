namespace RapidTransit
{
    using Autofac;
    using MassTransit.Courier;
    using Topshelf;


    public abstract class ExecuteActivityServiceBootstrapper<TActivity, TArguments> :
        IServiceBootstrapper
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _lifetimeScopeTag;
        readonly string _serviceName;

        protected ExecuteActivityServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _serviceName = typeof(TActivity).GetServiceDescription();
            _lifetimeScopeTag = string.Format("service_{0}", _serviceName);
        }

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public string LifetimeScopeTag
        {
            get { return _lifetimeScopeTag; }
        }

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<TActivity>();

            builder.RegisterType<AutofacExecuteActivityFactory<TActivity, TArguments>>()
                   .As<ExecuteActivityFactory<TArguments>>();

            builder.RegisterType<ExecuteActivityService<TActivity, TArguments>>()
                   .As<ServiceControl>();
        }
    }
}