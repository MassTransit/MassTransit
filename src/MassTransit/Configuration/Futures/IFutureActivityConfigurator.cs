namespace MassTransit
{
    using System;
    using System.Threading.Tasks;


    public interface IFutureActivityConfigurator
    {
        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior
        /// </summary>
        /// <param name="action">The synchronous delegate</param>
        void Then(Action<BehaviorContext<FutureState>> action);

        /// <summary>
        /// Adds an asynchronous delegate activity to the event's behavior
        /// </summary>
        /// <param name="action">The asynchronous delegate</param>
        void ThenAsync(Func<BehaviorContext<FutureState>, Task> action);
    }
}
