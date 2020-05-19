namespace MassTransit.StructureMapIntegration
{
    using StructureMap;


    public interface IConfigurationExpressionMediatorConfigurator :
        IMediatorConfigurator<IContext>
    {
        ConfigurationExpression Builder { get; }
    }
}
