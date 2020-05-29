namespace MassTransit
{
    using System;


    public interface IMediatorConfigurator<out TContainerContext> :
        IRegistrationConfigurator
        where TContainerContext : class
    {
        /// <summary>
        /// Optionally configure the pipeline used by the mediator
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureMediator(Action<TContainerContext, IReceiveEndpointConfigurator> configure);
    }
}
