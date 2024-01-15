namespace MassTransit.Tests.ContainerTests.Scenarios
{
    public class SimpleMessageClass :
        SimpleMessageInterface
    {
        public SimpleMessageClass(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
