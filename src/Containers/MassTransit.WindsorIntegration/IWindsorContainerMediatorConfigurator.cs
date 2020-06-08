namespace MassTransit.WindsorIntegration
{
    using Castle.Windsor;


    public interface IWindsorContainerMediatorConfigurator :
        IMediatorRegistrationConfigurator
    {
        IWindsorContainer Container { get; }
    }
}
