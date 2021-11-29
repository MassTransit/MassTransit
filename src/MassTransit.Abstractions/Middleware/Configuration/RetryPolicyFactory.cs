namespace MassTransit.Configuration
{
    public delegate IRetryPolicy RetryPolicyFactory(IExceptionFilter filter);
}
