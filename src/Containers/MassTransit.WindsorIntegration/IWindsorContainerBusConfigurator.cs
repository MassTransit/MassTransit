namespace MassTransit.WindsorIntegration
{
    using Castle.Windsor;


    public interface IWindsorContainerBusConfigurator :
        IBusRegistrationConfigurator
    {
        IWindsorContainer Container { get; }
    }
}
