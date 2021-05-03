namespace MassTransit.KafkaIntegration
{
    using Contexts;
    using GreenPipes;


    public interface ICheckpointPipeConfigurator :
        IPipeConfigurator<CheckpointContext>
    {
    }
}
