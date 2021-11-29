namespace MassTransit.DependencyInjection.Registration
{
    using Configuration;


    public interface IRegistrationFilter
    {
        bool Matches(IConsumerRegistration registration);
        bool Matches(ISagaRegistration registration);
        bool Matches(IExecuteActivityRegistration registration);
        bool Matches(IActivityRegistration registration);
        bool Matches(IFutureRegistration registration);
    }
}
