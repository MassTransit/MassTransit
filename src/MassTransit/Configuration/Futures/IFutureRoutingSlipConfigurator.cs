namespace MassTransit
{
    using System;
    using Courier.Contracts;
    using Futures;


    public interface IFutureRoutingSlipConfigurator<TResult, TFault, out TInput>
        where TResult : class
        where TFault : class
        where TInput : class
    {
        /// <summary>
        /// If specified, the routing slip is added to the pending results, using the routing slip tracking
        /// number. When the routing slip completes or faults, the pending result is completed or faulted.
        /// </summary>
        void TrackPendingRoutingSlip();

        /// <summary>
        /// Builds the routing slip itinerary when the command is received. The routing slip builder
        /// is passed, along with the <see cref="BehaviorContext{FutureState,TInput}" />. The tracking numbers,
        /// subscriptions, and FutureId variables are already initialized.
        /// </summary>
        /// <param name="buildItinerary"></param>
        void BuildItinerary(BuildItineraryCallback<TInput> buildItinerary);

        /// <summary>
        /// Builds the routing slip itinerary when the command is received using a container-registered
        /// <see cref="IItineraryPlanner{TInput}" />.
        /// </summary>
        void BuildUsingItineraryPlanner();

        /// <summary>
        /// Configure the behavior when the routing slip completes.
        /// </summary>
        /// <param name="configure"></param>
        void OnRoutingSlipCompleted(Action<IFutureResultConfigurator<TResult, RoutingSlipCompleted>> configure);

        /// <summary>
        /// Configure what happens when the routing slip faults
        /// </summary>
        /// <param name="configure"></param>
        void OnRoutingSlipFaulted(Action<IFutureFaultConfigurator<TFault, RoutingSlipFaulted>> configure);

        /// <summary>
        /// Add activities to the state machine that are executed when the routing slip is completed
        /// </summary>
        /// <param name="configure"></param>
        void WhenRoutingSlipCompleted(
            Func<EventActivityBinder<FutureState, RoutingSlipCompleted>, EventActivityBinder<FutureState, RoutingSlipCompleted>> configure);

        /// <summary>
        /// Add activities to the state machine that are executed when the routing slip is faulted
        /// </summary>
        /// <param name="configure"></param>
        void WhenRoutingSlipFaulted(Func<EventActivityBinder<FutureState, RoutingSlipFaulted>, EventActivityBinder<FutureState, RoutingSlipFaulted>> configure);
    }
}
