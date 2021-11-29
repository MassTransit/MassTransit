namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Observables;


    /// <summary>
    /// Converts an inbound context type to a pipe context type post-dispatch
    /// </summary>
    /// <typeparam name="TInput">The pipe context type</typeparam>
    /// <typeparam name="TOutput">The subsequent pipe context type</typeparam>
    public class OutputPipeFilter<TInput, TOutput> :
        IOutputPipeFilter<TInput, TOutput>
        where TInput : class, PipeContext
        where TOutput : class, TInput
    {
        readonly IPipeContextConverter<TInput, TOutput> _contextConverter;
        readonly FilterObservable<TOutput> _observers;
        readonly FilterObservable _outerObservers;
        readonly ITeeFilter<TOutput> _output;

        public OutputPipeFilter(IPipeContextConverter<TInput, TOutput> contextConverter, FilterObservable observers, ITeeFilter<TOutput> outputFilter)
        {
            _outerObservers = observers;
            _contextConverter = contextConverter;

            _output = outputFilter;

            _observers = new FilterObservable<TOutput>();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("dispatchPipe");
            scope.Add("outputType", TypeCache<TOutput>.ShortName);

            _output.Probe(scope);
        }

        Task IFilter<TInput>.Send(TInput context, IPipe<TInput> next)
        {
            return _contextConverter.TryConvert(context, out var pipeContext)
                ? SendToOutput(next, pipeContext)
                : next.Send(context);
        }

        ConnectHandle IFilterObserverConnector<TOutput>.ConnectObserver(IFilterObserver<TOutput> observer)
        {
            return _observers.Connect(observer);
        }

        ConnectHandle IPipeConnector<TOutput>.ConnectPipe(IPipe<TOutput> pipe)
        {
            return _output.ConnectPipe(pipe);
        }

        async Task SendToOutput(IPipe<TInput> next, TOutput pipeContext)
        {
            if (_observers.Count > 0)
            {
                var preSendTask = _observers.PreSend(pipeContext);
                if (preSendTask.Status != TaskStatus.RanToCompletion)
                    await preSendTask.ConfigureAwait(false);
            }

            if (_outerObservers.Count > 0)
            {
                var preSendTask = _outerObservers.PreSend(pipeContext);
                if (preSendTask.Status != TaskStatus.RanToCompletion)
                    await preSendTask.ConfigureAwait(false);
            }

            try
            {
                await _output.Send(pipeContext, next).ConfigureAwait(false);

                if (_observers.Count > 0)
                {
                    var postSendTask = _observers.PostSend(pipeContext);
                    if (postSendTask.Status != TaskStatus.RanToCompletion)
                        await postSendTask.ConfigureAwait(false);
                }

                if (_outerObservers.Count > 0)
                {
                    var postSendTask = _outerObservers.PostSend(pipeContext);
                    if (postSendTask.Status != TaskStatus.RanToCompletion)
                        await postSendTask.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (_observers.Count > 0)
                {
                    var sendFaultTask = _observers.SendFault(pipeContext, ex);
                    if (sendFaultTask.Status != TaskStatus.RanToCompletion)
                        await sendFaultTask.ConfigureAwait(false);
                }

                if (_outerObservers.Count > 0)
                {
                    var sendFaultTask = _outerObservers.SendFault(pipeContext, ex);
                    if (sendFaultTask.Status != TaskStatus.RanToCompletion)
                        await sendFaultTask.ConfigureAwait(false);
                }

                throw;
            }
        }
    }


    public class OutputPipeFilter<TInput, TOutput, TKey> :
        OutputPipeFilter<TInput, TOutput>,
        IOutputPipeFilter<TInput, TOutput, TKey>
        where TInput : class, PipeContext
        where TOutput : class, PipeContext, TInput
    {
        readonly ITeeFilter<TOutput, TKey> _outputFilter;

        public OutputPipeFilter(IPipeContextConverter<TInput, TOutput> contextConverter, FilterObservable observers, KeyAccessor<TInput, TKey> keyAccessor)
            : this(contextConverter, observers, new TeeFilter<TOutput, TKey>(keyAccessor))
        {
        }

        protected OutputPipeFilter(IPipeContextConverter<TInput, TOutput> contextConverter, FilterObservable observers, ITeeFilter<TOutput, TKey> outputFilter)
            : base(contextConverter, observers, outputFilter)
        {
            _outputFilter = outputFilter;
        }

        public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
            where T : class, PipeContext
        {
            return _outputFilter.ConnectPipe(key, pipe);
        }
    }
}
