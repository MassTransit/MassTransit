namespace MassTransit.Testing.Observers
{
    using GreenPipes;


    public interface IInactivityObservationSource
    {
        ConnectHandle ConnectInactivityObserver(IInactivityObserver observer);

        /// <summary>
        /// True if the inactivity source is currently inactive
        /// </summary>
        bool IsInactive { get; }
    }
}
