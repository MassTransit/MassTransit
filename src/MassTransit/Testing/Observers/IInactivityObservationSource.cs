namespace MassTransit.Testing.Observers
{
    using GreenPipes;


    public interface IInactivityObservationSource
    {
        /// <summary>
        /// True if the inactivity source is currently inactive
        /// </summary>
        bool IsInactive { get; }

        ConnectHandle ConnectInactivityObserver(IInactivityObserver observer);
    }
}
