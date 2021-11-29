namespace MassTransit
{
    using System.Threading.Tasks;


    /// <summary>
    /// Called by the future to build the routing slip
    /// </summary>
    /// <param name="context">The input consume context</param>
    /// <param name="builder">The routing slip itinerary builder</param>
    /// <typeparam name="TInput">The input message type</typeparam>
    public delegate Task BuildItineraryCallback<in TInput>(BehaviorContext<FutureState, TInput> context, IItineraryBuilder builder)
        where TInput : class;
}
