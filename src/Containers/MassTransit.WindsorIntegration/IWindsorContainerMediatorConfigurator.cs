namespace MassTransit.WindsorIntegration
{
    using Castle.MicroKernel;
    using Castle.Windsor;


    public interface IWindsorContainerMediatorConfigurator :
        IMediatorConfigurator<IKernel>
    {
        IWindsorContainer Container { get; }
    }
}
