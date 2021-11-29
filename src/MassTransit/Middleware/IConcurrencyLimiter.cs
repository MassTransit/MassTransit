namespace MassTransit.Middleware
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;


    public interface IConcurrencyLimiter :
        IConsumer<SetConcurrencyLimit>
    {
        int Available { get; }
        int Limit { get; }

        Task Wait(CancellationToken cancellationToken);

        void Release();
    }
}
