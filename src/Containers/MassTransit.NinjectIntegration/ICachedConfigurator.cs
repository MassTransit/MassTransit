namespace MassTransit.NinjectIntegration
{
    using Ninject;


    public interface ICachedConfigurator
    {
        void Configure(IReceiveEndpointConfigurator configurator, IKernel kernel);
    }
}
