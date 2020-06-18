namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using MassTransit.Registration;


    public interface IContainerBuilderRiderConfigurator :
        IRiderRegistrationConfigurator
    {
        ContainerBuilder Builder { get; }
    }
}
