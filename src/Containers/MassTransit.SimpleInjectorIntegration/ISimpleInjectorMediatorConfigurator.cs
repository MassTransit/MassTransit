namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using SimpleInjector;


    public interface ISimpleInjectorMediatorConfigurator :
        IMediatorConfigurator<Container>
    {
        Container Container { get; }

        /// <summary>
        /// Optionally configure the pipeline used by the mediator
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureMediator(Action<IReceiveEndpointConfigurator> configure);
    }
}
