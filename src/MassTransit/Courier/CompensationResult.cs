namespace MassTransit.Courier
{
    using System;
    using System.Threading.Tasks;


    public interface CompensationResult
    {
        Task Evaluate();

        bool IsFailed(out Exception exception);
    }
}
