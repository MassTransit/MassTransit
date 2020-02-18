namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.Windsor;


    public interface IWindsorContainerConfigurator :
        IRegistrationConfigurator<IKernel>
    {
        IWindsorContainer Container { get; }
    }
}
