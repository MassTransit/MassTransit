namespace MassTransit.Testing.Implementations
{
    using System.Threading.Tasks;


    public interface IInactivityObserver
    {
        void Connected(IInactivityObservationSource source);

        Task NoActivity();

        void ForceInactive();
    }
}
