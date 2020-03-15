namespace MassTransit.Definition
{
    public interface IDefinition
    {
        int? ConcurrentMessageLimit { get; }
    }
}
