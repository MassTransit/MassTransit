namespace MassTransit.Middleware
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;


    /// <summary>
    /// Rescue catches an exception, and if the exception matches the exception filter,
    /// passes control to the rescue pipe.
    /// </summary>
    /// <typeparam name="TContext">The context type</typeparam>
    /// <typeparam name="TRescueContext"></typeparam>
    public class RescueFilter<TContext, TRescueContext> :
        IFilter<TContext>
        where TContext : class, PipeContext
        where TRescueContext : class, PipeContext
    {
        readonly IExceptionFilter _exceptionFilter;
        readonly RescueContextFactory<TContext, TRescueContext> _rescueContextFactory;
        readonly IPipe<TRescueContext> _rescuePipe;

        public RescueFilter(IPipe<TRescueContext> rescuePipe, IExceptionFilter exceptionFilter,
            RescueContextFactory<TContext, TRescueContext> rescueContextFactory)
        {
            _rescuePipe = rescuePipe;
            _exceptionFilter = exceptionFilter;
            _rescueContextFactory = rescueContextFactory;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("rescue");

            _rescuePipe.Probe(scope);
        }

        [DebuggerNonUserCode]
        async Task IFilter<TContext>.Send(TContext context, IPipe<TContext> next)
        {
            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                if (!_exceptionFilter.Match(ex.GetBaseException()))
                    throw;

                var rescueContext = _rescueContextFactory(context, ex);

                await _rescuePipe.Send(rescueContext).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!_exceptionFilter.Match(ex))
                    throw;

                var rescueContext = _rescueContextFactory(context, ex);

                await _rescuePipe.Send(rescueContext).ConfigureAwait(false);
            }
        }
    }
}
