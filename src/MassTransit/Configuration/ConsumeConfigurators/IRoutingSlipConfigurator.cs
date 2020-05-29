namespace MassTransit.ConsumeConfigurators
{
    using Courier.Contracts;
    using GreenPipes;


    /// <summary>
    /// Configure a message handler, including specifying filters that are executed around
    /// the handler itself
    /// </summary>
    public interface IRoutingSlipConfigurator :
        IConsumeConfigurator,
        IPipeConfigurator<ConsumeContext<RoutingSlip>>
    {
    }
}
