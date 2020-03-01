namespace MassTransit.Configuration
{
    using GreenPipes;
    using GreenPipes.Builders;
    using Pipeline;


    public interface IReceivePipeConfiguration
    {
        ISpecification Specification { get; }
        IReceivePipeConfigurator Configurator { get; }
        IBuildPipeConfigurator<ReceiveContext> DeadLetterConfigurator { get; }
        IBuildPipeConfigurator<ExceptionReceiveContext> ErrorConfigurator { get; }

        IReceivePipe CreatePipe(IConsumePipe consumePipe, IMessageDeserializer messageDeserializer);
    }
}
