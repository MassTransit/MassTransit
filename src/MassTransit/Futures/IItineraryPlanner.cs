namespace MassTransit.Futures
{
    using System.Threading.Tasks;
    using Courier;


    /// <summary>
    /// Implement to build a routing slip. This can be resolved by a durable future to build
    /// a routing slip at runtime in response to an input command.
    /// </summary>
    /// <typeparam name="TInput">The input message type</typeparam>
    public interface IItineraryPlanner<in TInput>
        where TInput : class
    {
        Task PlanItinerary(FutureConsumeContext<TInput> value, ItineraryBuilder builder);
    }
}
