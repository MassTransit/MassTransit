namespace RapidTransit
{
    using System;
    using Autofac;
    using Topshelf;


    public abstract class ServiceBusInstanceServiceBootstrapper :
        IServiceBootstrapper
    {
        readonly ILifetimeScope _lifetimeScope;
        readonly string _lifetimeScopeTag;
        readonly string _serviceName;

        protected ServiceBusInstanceServiceBootstrapper(ILifetimeScope lifetimeScope, Type bootstrapperType)
        {
            _lifetimeScope = lifetimeScope;
            _serviceName = bootstrapperType.GetServiceDescription();
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
            ILifetimeScope lifetimeScope = _lifetimeScope.BeginLifetimeScope(_lifetimeScopeTag, ConfigureLifetimeScope);

            var serviceControl = lifetimeScope.Resolve<ServiceControl>();

            return new LifetimeScopeServiceControl(lifetimeScope, serviceControl, _serviceName);
        }

        protected virtual void ConfigureLifetimeScope(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitMqBusInstanceService>()
                   .InstancePerServiceScope(this)
                   .WithParameter(TypedParameter.From(_serviceName))
                   .As<ServiceControl>();
        }
    }
}