namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Middleware;


    public static class HostConfigurationRetryExtensions
    {
        public static async Task Retry(this IHostConfiguration hostConfiguration, Func<Task> factory, CancellationToken cancellationToken,
            CancellationToken stoppingToken)
        {
            var description = hostConfiguration.HostAddress;

            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, stoppingToken);

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

                        if (stoppingToken.IsCancellationRequested)
                            throw new ConnectionException($"The transport is stopping and cannot be used: {description}", retryContext?.Exception);
                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

                        await factory().ConfigureAwait(false);
                        return;
                    }
                    catch (OperationCanceledException exception) when (exception.CancellationToken == stoppingToken)
                    {
                        throw new ConnectionException($"The transport is stopping and cannot be used: {description}", retryContext?.Exception);
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

                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                throw new ConnectionException($"The transport is stopping and cannot be used: {description}", retryContext?.Exception);
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
