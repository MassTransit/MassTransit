namespace MassTransit.Testing.Observers
{
    using System.Threading.Tasks;


    public interface IInactivityObserver
    {
        Task NoActivity();
    }
}
