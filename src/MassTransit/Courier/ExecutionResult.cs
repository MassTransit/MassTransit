namespace MassTransit.Courier
{
    using System;
    using System.Threading.Tasks;


    public interface ExecutionResult
    {
        Task Evaluate();

        bool IsFaulted(out Exception exception);
    }
}
