namespace MassTransit.Containers.Tests.Scenarios
{
    public interface AnotherMessageConsumer :
        IConsumer<AnotherMessageInterface>
    {
        AnotherMessageInterface Last { get; }
    }
}
