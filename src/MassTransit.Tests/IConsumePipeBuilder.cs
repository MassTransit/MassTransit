namespace MassTransit.Tests
{
    using GreenPipes;


    public interface IConsumePipeBuilder :
        IPipeBuilder<ConsumeContext>
    {
    }
}
