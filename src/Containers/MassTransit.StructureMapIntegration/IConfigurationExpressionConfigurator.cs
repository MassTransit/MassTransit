namespace MassTransit.StructureMapIntegration
{
    using StructureMap;


    public interface IConfigurationExpressionConfigurator :
        IRegistrationConfigurator<IContext>
    {
        ConfigurationExpression Builder { get; }
    }
}
