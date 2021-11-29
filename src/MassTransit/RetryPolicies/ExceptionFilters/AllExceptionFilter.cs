namespace MassTransit.RetryPolicies.ExceptionFilters
{
    using System;


    public class AllExceptionFilter :
        IExceptionFilter
    {
        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateScope("all");
        }

        bool IExceptionFilter.Match(Exception exception)
        {
            return true;
        }
    }
}
