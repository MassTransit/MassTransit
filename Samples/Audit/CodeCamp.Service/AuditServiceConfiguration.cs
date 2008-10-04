namespace CodeCamp.Service
{
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class AuditServiceConfiguration :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public AuditServiceConfiguration(string xmlFile)
        {
            _lifecycle = new AuditServiceLifeCycle(xmlFile);
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