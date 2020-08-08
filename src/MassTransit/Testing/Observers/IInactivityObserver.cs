namespace MassTransit.Testing.Observers
{
    using System.Threading.Tasks;


    public interface IInactivityObserver
    {
        void Connected(IInactivityObservationSource source);

        Task NoActivity();
    }
}
