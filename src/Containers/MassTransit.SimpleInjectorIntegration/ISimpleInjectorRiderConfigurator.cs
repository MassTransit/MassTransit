namespace MassTransit.SimpleInjectorIntegration
{
    using MassTransit.Registration;
    using SimpleInjector;


    public interface  ISimpleInjectorRiderConfigurator :
        IRiderRegistrationConfigurator
    {
        Container Container { get; }
    }
}
