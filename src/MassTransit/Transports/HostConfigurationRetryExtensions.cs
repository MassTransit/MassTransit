namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Middleware;


    public static class HostConfigurationRetryExtensions
    {
        public static async Task Retry(this IHostConfiguration hostConfiguration, Func<Task> factory, ISupervisor supervisor,
            CancellationToken cancellationToken)
        {
            var description = hostConfiguration.HostAddress;

            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, supervisor.Stopping);

            var stoppingContext = new SupervisorStoppingContext(tokenSource.Token);

            RetryPolicyContext<SupervisorStoppingContext> policyContext = hostConfiguration.ReceiveTransportRetryPolicy.CreatePolicyContext(stoppingContext);

            try
            {
                RetryContext<SupervisorStoppingContext> retryContext = null;

                while (!tokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (retryContext?.Delay != null)
                            await Task.Delay(retryContext.Delay.Value, tokenSource.Token).ConfigureAwait(false);

                        if (tokenSource.Token.IsCancellationRequested)
                            throw new ConnectionException($"The connection is stopping and cannot be used: {description}", retryContext?.Exception);

                        await factory().ConfigureAwait(false);
                        return;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        if (retryContext != null)
                        {
                            retryContext = retryContext.CanRetry(exception, out RetryContext<SupervisorStoppingContext> nextRetryContext)
                                ? nextRetryContext
                                : null;
                        }

                        if (retryContext == null && !policyContext.CanRetry(exception, out retryContext))
                            throw;
                    }

                    if (tokenSource.Token.IsCancellationRequested)
                        break;

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), tokenSource.Token).ConfigureAwait(false);
                    }
                    catch
                    {
                        // just a little breather before reconnecting the receive transport
                    }
                }

                throw new ConnectionException($"The connection is stopping and cannot be used: {description}", retryContext?.Exception);
            }
            finally
            {
                policyContext.Dispose();
            }
        }


        class SupervisorStoppingContext :
            BasePipeContext
        {
            public SupervisorStoppingContext(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
            }
        }
    }
}
