namespace MassTransit.StructureMapIntegration
{
    using StructureMap;


    public interface IConfigurationExpressionMediatorConfigurator :
        IMediatorRegistrationConfigurator<IContext>
    {
        ConfigurationExpression Builder { get; }
    }
}
