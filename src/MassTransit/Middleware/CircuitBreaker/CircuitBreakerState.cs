namespace MassTransit.Middleware.CircuitBreaker
{
    using System;
    using System.Threading.Tasks;


    public interface ICircuitBreakerBehavior :
        IProbeSite
    {
        Task PreSend();
        Task PostSend();
        Task SendFault(Exception exception);
    }
}
