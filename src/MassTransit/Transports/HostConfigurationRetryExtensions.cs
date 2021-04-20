namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using GreenPipes;
    using GreenPipes.Agents;


    public static class HostConfigurationRetryExtensions
    {
        public static async Task Retry(this IHostConfiguration hostConfiguration, Func<Task> factory, ISupervisor supervisor)
        {
            var description = hostConfiguration.HostAddress;

            var stoppingContext = new SupervisorStoppingContext(supervisor.Stopping);

            RetryPolicyContext<SupervisorStoppingContext> policyContext = hostConfiguration.ReceiveTransportRetryPolicy.CreatePolicyContext(stoppingContext);

            try
            {
                RetryContext<SupervisorStoppingContext> retryContext = null;

                while (!supervisor.Stopping.IsCancellationRequested)
                {
                    try
                    {
                        if (retryContext?.Delay != null)
                            await Task.Delay(retryContext.Delay.Value, supervisor.Stopping).ConfigureAwait(false);

                        if (supervisor.Stopping.IsCancellationRequested)
                        {
                            throw new OperationCanceledException($"The connection is stopping and cannot be used: {description}", retryContext?.Exception,
                                supervisor.Stopping);
                        }

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

                    if (supervisor.Stopping.IsCancellationRequested)
                        break;

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1), supervisor.Stopping).ConfigureAwait(false);
                    }
                    catch
                    {
                        // just a little breather before reconnecting the receive transport
                    }
                }

                throw new OperationCanceledException($"The connection is stopping and cannot be used: {description}", retryContext?.Exception,
                    supervisor.Stopping);
            }
            finally
            {
                policyContext.Dispose();
            }
        }


        class SupervisorStoppingContext :
            BasePipeContext,
            PipeContext
        {
            public SupervisorStoppingContext(CancellationToken cancellationToken)
                : base(cancellationToken)
            {
            }
        }
    }
}
