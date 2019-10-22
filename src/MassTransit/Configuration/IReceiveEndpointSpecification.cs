namespace MassTransit
{
    using GreenPipes;


    /// <summary>
    /// Specification for configuring a receive endpoint
    /// </summary>
    public interface IReceiveEndpointSpecification :
        ISpecification
    {
        void Configure(IReceiveEndpointBuilder builder);
    }
}
