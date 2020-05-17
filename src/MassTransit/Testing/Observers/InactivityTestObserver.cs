namespace MassTransit.Testing.Observers
{
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public abstract class InactivityTestObserver :
        Connectable<IInactivityObserver>,
        IInactivityObservationSource
    {
        public ConnectHandle ConnectInactivityObserver(IInactivityObserver observer)
        {
            return Connect(observer);
        }

        public abstract bool IsInactive { get; }

        protected Task NotifyInactive()
        {
            return ForEachAsync(x => x.NoActivity());
        }
    }
}
