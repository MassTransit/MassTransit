namespace MassTransit
{
    public interface IDefinition
    {
        int? ConcurrentMessageLimit { get; }
    }
}
