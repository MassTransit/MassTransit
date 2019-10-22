namespace MassTransit.Host.Activities
{
    using Autofac;
    using AutofacIntegration;
    using Courier;
    using Topshelf;


    public abstract class ActivityServiceBootstrapper<TActivity, TArguments, TLog> :
        IServiceBootstrapper
        where TActivity : class, IActivity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _serviceName;

        protected ActivityServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _serviceName = typeof(TActivity).GetServiceDescription();

            LifetimeScopeTag = $"service_{_serviceName}";
        }

        public ServiceControl CreateService()
        {
            var lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        public string ServiceName => _serviceName;

        public string LifetimeScopeTag { get; }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<TActivity>();

            builder.RegisterType<AutofacExecuteActivityFactory<TActivity, TArguments>>()
                .As<IExecuteActivityFactory<TActivity, TArguments>>();

            builder.RegisterType<AutofacCompensateActivityFactory<TActivity, TLog>>()
                .As<ICompensateActivityFactory<TActivity, TLog>>();

            builder.RegisterType<ActivityService<TActivity, TArguments, TLog>>()
                .As<ServiceControl>();
        }
    }
}
