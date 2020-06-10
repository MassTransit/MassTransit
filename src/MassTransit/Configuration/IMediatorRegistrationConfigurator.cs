namespace MassTransit
{
    using System;
    using Registration;


    public interface IMediatorRegistrationConfigurator :
        IRegistrationConfigurator
    {
        /// <summary>
        /// Optionally configure the pipeline used by the mediator
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureMediator(Action<IMediatorRegistrationContext, IReceiveEndpointConfigurator> configure);
    }
}
