namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Contracts;
    using Exceptions;
    using GreenPipes;
    using Logging;
    using Metadata;


    public class ExecuteActivityHost<TActivity, TArguments> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly Uri _compensateAddress;
        readonly IPipe<ExecuteContext<TArguments>> _executePipe;

        public ExecuteActivityHost(IPipe<ExecuteContext<TArguments>> executePipe, Uri compensateAddress)
        {
            _executePipe = executePipe;
            _compensateAddress = compensateAddress;
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            var activity = LogContext.IfEnabled(OperationName.Courier.Execute)?.StartActivity(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                ArgumentType = TypeMetadataCache<TArguments>.ShortName
            });

            activity?.AddBaggage(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber);

            var timer = Stopwatch.StartNew();
            try
            {
                ExecuteContext<TArguments> executeContext = new HostExecuteContext<TArguments>(_compensateAddress, context);

                LogContext.Debug?.Log("Execute Activity: {TrackingNumber} ({Activity}, {Host})", executeContext.TrackingNumber,
                    TypeMetadataCache<TActivity>.ShortName, context.ReceiveContext.InputAddress);

                try
                {
                    await Task.Yield();
                    await _executePipe.Send(executeContext).ConfigureAwait(false);

                    var result = executeContext.Result
                        ?? executeContext.Faulted(new ActivityExecutionException("The activity execute did not return a result"));

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await executeContext.Faulted(ex).Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, exception).ConfigureAwait(false);

                if (exception.CancellationToken == context.CancellationToken)
                    throw;

                throw new ConsumerCanceledException($"The operation was cancelled by the activity: {TypeMetadataCache<TActivity>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, ex).ConfigureAwait(false);

                LogContext.Error?.Log(ex, "Activity {Activity} execution faulted: {Exception}", TypeMetadataCache<TActivity>.ShortName);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("executeActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                ArgumentType = TypeMetadataCache<TArguments>.ShortName
            });

            if (_compensateAddress != null)
                scope.Add("compensateAddress", _compensateAddress);

            _executePipe.Probe(scope);
        }
    }
}
