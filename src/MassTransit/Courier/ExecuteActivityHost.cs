namespace MassTransit.Courier
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;


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
            var timer = Stopwatch.StartNew();

            StartedActivity? activity = LogContext.Current?.StartExecuteActivity<TActivity, TArguments>(context);
            StartedInstrument? instrument = LogContext.Current?.StartActivityExecuteInstrument<TActivity, TArguments>(context, timer);

            try
            {
                ExecuteContext<TArguments> executeContext = new HostExecuteContext<TArguments>(_compensateAddress, context);

                LogContext.Debug?.Log("Execute Activity: {TrackingNumber} ({Activity}, {Host})", executeContext.TrackingNumber,
                    TypeCache<TActivity>.ShortName, context.ReceiveContext.InputAddress);

                try
                {
                    await _executePipe.Send(executeContext).ConfigureAwait(false);

                    var result = executeContext.Result
                        ?? executeContext.Faulted(new ActivityExecutionException("The activity execute did not return a result"));

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    if (executeContext.Result == null || !executeContext.Result.IsFaulted(out var faultException) || faultException != exception)
                        executeContext.Result = executeContext.Faulted(exception);

                    await context.NotifyFaulted(timer.Elapsed, TypeCache<TActivity>.ShortName, exception).ConfigureAwait(false);

                    activity?.AddExceptionEvent(exception);
                    instrument?.AddException(exception);

                    await executeContext.Result.Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception exception) when ((exception is OperationCanceledException || exception.GetBaseException() is OperationCanceledException)
                                              && !context.CancellationToken.IsCancellationRequested)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<TActivity>.ShortName, exception).ConfigureAwait(false);

                activity?.AddExceptionEvent(exception);

                instrument?.AddException(exception);

                throw new ConsumerCanceledException($"The operation was canceled by the activity: {TypeCache<TActivity>.ShortName}");
            }
            catch (Exception exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<TActivity>.ShortName, exception).ConfigureAwait(false);

                activity?.AddExceptionEvent(exception);

                instrument?.AddException(exception);

                throw;
            }
            finally
            {
                activity?.Stop();
                instrument?.Stop();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("executeActivity");
            scope.Set(new
            {
                ActivityType = TypeCache<TActivity>.ShortName,
                ArgumentType = TypeCache<TArguments>.ShortName
            });

            if (_compensateAddress != null)
                scope.Add("compensateAddress", _compensateAddress);

            _executePipe.Probe(scope);
        }
    }
}
