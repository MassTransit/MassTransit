namespace MassTransit.Futures
{
    using System.Threading.Tasks;
    using Courier;


    /// <summary>
    /// Called by the future to build the routing slip
    /// </summary>
    /// <param name="context">The input consume context</param>
    /// <param name="builder">The routing slip itinerary builder</param>
    /// <typeparam name="TInput">The input message type</typeparam>
    public delegate Task BuildItineraryCallback<in TInput>(FutureConsumeContext<TInput> context, ItineraryBuilder builder)
        where TInput : class;
}
