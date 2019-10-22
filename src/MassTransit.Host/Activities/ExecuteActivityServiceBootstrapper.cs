namespace MassTransit.Host.Activities
{
    using Autofac;
    using AutofacIntegration;
    using Courier;
    using Topshelf;


    public abstract class ExecuteActivityServiceBootstrapper<TActivity, TArguments> :
        IServiceBootstrapper
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _lifetimeScopeTag;
        readonly string _serviceName;

        protected ExecuteActivityServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _serviceName = typeof(TActivity).GetServiceDescription();
            _lifetimeScopeTag = $"service_{_serviceName}";
        }

        public string ServiceName => _serviceName;

        public string LifetimeScopeTag => _lifetimeScopeTag;

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
                .As<IExecuteActivityFactory<TActivity, TArguments>>();

            builder.RegisterType<ExecuteActivityService<TActivity, TArguments>>()
                .As<ServiceControl>();
        }
    }
}
