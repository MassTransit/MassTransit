namespace MassTransit.Middleware
{
    using System.Threading.Tasks;


    /// <summary>
    /// Retains the last value that was sent through the filter, usable as a source to a join pipe
    /// </summary>
    public class LatestFilter<T> :
        IFilter<T>,
        ILatestFilter<T>
        where T : class, PipeContext
    {
        readonly TaskCompletionSource<bool> _hasValue;
        T _latest;

        public LatestFilter()
        {
            _hasValue = new TaskCompletionSource<bool>(TaskCreationOptions.None | TaskCreationOptions.RunContinuationsAsynchronously);
        }

        Task IFilter<T>.Send(T context, IPipe<T> next)
        {
            _latest = context;
            _hasValue.TrySetResult(true);

            return next.Send(context);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.CreateFilterScope("latest");
        }

        Task<T> ILatestFilter<T>.Latest => GetLatest();

        async Task<T> GetLatest()
        {
            await _hasValue.Task.ConfigureAwait(false);

            return _latest;
        }
    }
}
