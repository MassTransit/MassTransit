namespace RapidTransit
{
    using Autofac;
    using MassTransit.Courier;
    using Topshelf;


    public abstract class ActivityServiceBootstrapper<TActivity, TArguments, TLog> :
        IServiceBootstrapper
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _serviceName;
        readonly string _lifetimeScopeTag;

        protected ActivityServiceBootstrapper(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;

            _serviceName = typeof(TActivity).GetServiceDescription();
            _lifetimeScopeTag = string.Format("service_{0}", _serviceName);
        }

        public ServiceControl CreateService()
        {
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();


            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public string LifetimeScopeTag
        {
            get { return _lifetimeScopeTag; }
        }


        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<TActivity>();

            builder.RegisterType<AutofacExecuteActivityFactory<TActivity, TArguments>>()
                   .As<ExecuteActivityFactory<TArguments>>();

            builder.RegisterType<AutofacCompensateActivityFactory<TActivity, TLog>>()
                   .As<CompensateActivityFactory<TLog>>();

            builder.RegisterType<ActivityService<TActivity, TArguments, TLog>>()
                   .As<ServiceControl>();
        }
    }
}