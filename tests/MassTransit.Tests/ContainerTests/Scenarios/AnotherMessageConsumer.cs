namespace MassTransit.Tests.ContainerTests.Scenarios
{
    public interface AnotherMessageConsumer :
        IConsumer<AnotherMessageInterface>
    {
        AnotherMessageInterface Last { get; }
    }
}
