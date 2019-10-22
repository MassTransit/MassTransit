namespace MassTransit.StructureMapIntegration
{
    using System;
    using StructureMap;


    public interface IConfigurationExpressionConfigurator :
        IRegistrationConfigurator
    {
        ConfigurationExpression Builder { get; }

        /// <summary>
        /// Add the bus to the container, configured properly
        /// </summary>
        /// <param name="busFactory"></param>
        void AddBus(Func<IContext, IBusControl> busFactory);
    }
}
