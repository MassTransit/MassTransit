namespace MassTransit.Configuration
{
    public interface IRedeliveryPipeSpecification
    {
        RedeliveryOptions Options { get; set; }
    }
}
