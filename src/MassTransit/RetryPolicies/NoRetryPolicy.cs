namespace MassTransit.RetryPolicies
{
    using System;


    public class NoRetryPolicy :
        IRetryPolicy
    {
        readonly IExceptionFilter _filter;

        public NoRetryPolicy(IExceptionFilter filter)
        {
            _filter = filter;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Set(new
            {
                Policy = "None"
            });
        }

        RetryPolicyContext<T> IRetryPolicy.CreatePolicyContext<T>(T context)
        {
            return new NoRetryPolicyContext<T>(this, context);
        }

        bool IRetryPolicy.IsHandled(Exception exception)
        {
            return _filter.Match(exception);
        }

        public override string ToString()
        {
            return "None";
        }
    }
}
