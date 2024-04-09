namespace MassTransit
{
    using System;

    public interface IFutureActivityConfigurator
    {
        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <param name="action">The synchronous delegate</param>
        void Then(Action<BehaviorContext<FutureState>> action);
    }
}
