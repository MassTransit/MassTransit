namespace PostalService.Host
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using Microsoft.Practices.ServiceLocation;

    public class PostalServiceConfiguration :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifeCycle;

        public PostalServiceConfiguration(IServiceLocator serviceLocator)
        {
            _lifeCycle = new PostalServiceLifeCycle(serviceLocator);
        }

        public override string ServiceName
        {
            get { return "PostalService"; }
        }

        public override string DisplayName
        {
            get { return "Sample Email Service"; }
        }

        public override string Description
        {
            get { return "We goin' postal!"; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifeCycle; }
        }
    }
}