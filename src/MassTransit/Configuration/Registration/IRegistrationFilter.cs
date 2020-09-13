namespace MassTransit.Registration
{
    public interface IRegistrationFilter
    {
        bool Matches(IConsumerRegistration registration);
        bool Matches(ISagaRegistration registration);
        bool Matches(IExecuteActivityRegistration registration);
        bool Matches(IActivityRegistration registration);
    }
}
