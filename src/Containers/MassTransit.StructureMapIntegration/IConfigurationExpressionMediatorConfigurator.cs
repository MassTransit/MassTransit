namespace MassTransit.StructureMapIntegration
{
    using StructureMap;


    public interface IConfigurationExpressionMediatorConfigurator :
        IMediatorRegistrationConfigurator
    {
        ConfigurationExpression Builder { get; }
    }
}
