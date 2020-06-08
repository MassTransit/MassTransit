namespace MassTransit.StructureMapIntegration
{
    using StructureMap;


    public interface IConfigurationExpressionBusConfigurator :
        IBusRegistrationConfigurator
    {
        ConfigurationExpression Builder { get; }
    }
}
