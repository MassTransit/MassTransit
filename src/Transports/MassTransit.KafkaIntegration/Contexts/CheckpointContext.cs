namespace MassTransit.KafkaIntegration.Contexts
{
    using GreenPipes;


    public interface CheckpointContext :
        PipeContext
    {
        string Topic { get; }
        int Partition { get; }
        long Offset { get; }
    }
}
