namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.Windsor;


    public interface IWindsorContainerMediatorConfigurator :
        IMediatorRegistrationConfigurator<IKernel>
    {
        IWindsorContainer Container { get; }
    }
}
