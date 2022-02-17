namespace MassTransit
{
    using Courier.Contracts;


    public interface IRoutingSlipBuilder :
        IItineraryBuilder
    {
        /// <summary>
        /// Builds the routing slip using the current state of the builder
        /// </summary>
        /// <returns>The RoutingSlip</returns>
        RoutingSlip Build();
    }
}
