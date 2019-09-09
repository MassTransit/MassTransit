namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.MicroKernel;
    using Castle.Windsor;


    public interface IWindsorContainerConfigurator :
        IRegistrationConfigurator
    {
        IWindsorContainer Container { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IKernel, IBusControl> busFactory);
    }
}
