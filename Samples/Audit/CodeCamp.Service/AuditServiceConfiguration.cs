namespace CodeCamp.Service
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using Microsoft.Practices.ServiceLocation;

    public class AuditServiceConfiguration :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public AuditServiceConfiguration(IServiceLocator serviceLocator)
        {
            _lifecycle = new AuditServiceLifeCycle(serviceLocator);
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }

        public override string ServiceName
        {
            get { return "Audit"; }
        }

        public override string DisplayName
        {
            get { return "Audit"; }
        }

        public override string Description
        {
            get { return "Audit"; }
        }
    }
}