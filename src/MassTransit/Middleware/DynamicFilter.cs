namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Observables;


    /// <summary>
    /// Dispatches an inbound pipe to one or more output pipes based on a dispatch
    /// type.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public class DynamicFilter<TInput> :
        IDynamicFilter<TInput>
        where TInput : class, PipeContext
    {
        readonly IPipe<TInput> _empty;
        readonly Dictionary<Type, IOutputFilter> _outputPipes;
        protected readonly IPipeContextConverterFactory<TInput> ConverterFactory;
        protected readonly FilterObservable Observers;

        IOutputFilter[] _outputPipeArray;

        public DynamicFilter(IPipeContextConverterFactory<TInput> converterFactory)
        {
            ConverterFactory = converterFactory;

            _outputPipes = new Dictionary<Type, IOutputFilter>();
            _outputPipeArray = Array.Empty<IOutputFilter>();

            Observers = new FilterObservable();
            _empty = Pipe.Empty<TInput>();
        }

        ConnectHandle IFilterObserverConnector.ConnectObserver<T>(IFilterObserver<T> observer)
        {
            return GetPipe<T>().ConnectObserver(observer);
        }

        ConnectHandle IFilterObserverConnector.ConnectObserver(IFilterObserver observer)
        {
            return Observers.Connect(observer);
        }

        public ConnectHandle ConnectPipe<T>(IPipe<T> pipe)
            where T : class, PipeContext
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            IPipeConnector<T> pipeConnector = GetPipe<T, IPipeConnector<T>>();

            return pipeConnector.ConnectPipe(pipe);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var pipe in _outputPipes.Values)
                pipe.Probe(context);
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public Task Send(TInput context, IPipe<TInput> next)
        {
            IOutputFilter[] outputPipes = _outputPipeArray;

            if (outputPipes.Length == 0)
                return Task.CompletedTask;

            if (outputPipes.Length == 1)
                return outputPipes[0].Send(context, next);

            async Task SendAsync()
            {
                var outputTasks = new List<Task>(outputPipes.Length);
                for (var i = 0; i < outputPipes.Length; i++)
                {
                    var outputTask = outputPipes[i].Send(context, _empty);
                    if (outputTask.Status == TaskStatus.RanToCompletion)
                        continue;

                    outputTasks.Add(outputTask);
                }

                await Task.WhenAll(outputTasks).ConfigureAwait(false);
                await next.Send(context).ConfigureAwait(false);
            }

            return SendAsync();
        }

        protected TResult GetPipe<T, TResult>()
            where T : class, PipeContext
            where TResult : class
        {
            return GetPipe<T>().As<TResult>();
        }

        protected IOutputFilter GetPipe<T>()
            where T : class, PipeContext
        {
            lock (_outputPipes)
            {
                if (_outputPipes.TryGetValue(typeof(T), out var outputPipe))
                    return outputPipe;

                outputPipe = CreateOutputPipe<T>();

                _outputPipes.Add(typeof(T), outputPipe);

                _outputPipeArray = _outputPipes.Values.ToArray();

                return outputPipe;
            }
        }

        protected virtual IOutputFilter CreateOutputPipe<T>()
            where T : class, PipeContext
        {
            IPipeContextConverter<TInput, T> converter = ConverterFactory.GetConverter<T>();

            return (IOutputFilter)Activator.CreateInstance(typeof(OutputFilter<>).MakeGenericType(typeof(TInput), typeof(T)), Observers, converter);
        }


        protected interface IOutputFilter :
            IFilter<TInput>,
            IFilterObserverConnector
        {
            TResult As<TResult>()
                where TResult : class;
        }


        protected class OutputFilter<TOutput> :
            IOutputFilter
            where TOutput : class, TInput
        {
            protected readonly IPipeContextConverter<TInput, TOutput> ContextConverter;
            protected readonly FilterObservable Observers;

            public OutputFilter(FilterObservable observers, IPipeContextConverter<TInput, TOutput> contextConverter)
            {
                ContextConverter = contextConverter;
                Observers = observers;

                Filter = new OutputPipeFilter<TInput, TOutput>(ContextConverter, Observers, new TeeFilter<TOutput>());
            }

            protected virtual IOutputPipeFilter<TInput, TOutput> Filter { get; }

            TResult IOutputFilter.As<TResult>()
            {
                return Filter as TResult;
            }

            ConnectHandle IFilterObserverConnector.ConnectObserver<T>(IFilterObserver<T> observer)
            {
                if (Filter is IFilterObserverConnector<T> connector)
                    return connector.ConnectObserver(observer);

                throw new ArgumentException($"The filter is not of the specified type: {typeof(T).Name}", nameof(observer));
            }

            public ConnectHandle ConnectObserver(IFilterObserver observer)
            {
                return Observers.Connect(observer);
            }

            public Task Send(TInput context, IPipe<TInput> next)
            {
                return Filter.Send(context, next);
            }

            public void Probe(ProbeContext context)
            {
                Filter.Probe(context);
            }
        }
    }


    public class DynamicFilter<TInput, TKey> :
        DynamicFilter<TInput>,
        IDynamicFilter<TInput, TKey>
        where TInput : class, PipeContext
    {
        readonly KeyAccessor<TInput, TKey> _keyAccessor;

        public DynamicFilter(IPipeContextConverterFactory<TInput> converterFactory, KeyAccessor<TInput, TKey> keyAccessor)
            : base(converterFactory)
        {
            _keyAccessor = keyAccessor;
        }

        public ConnectHandle ConnectPipe<T>(TKey key, IPipe<T> pipe)
            where T : class, PipeContext
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            IKeyPipeConnector<TKey> pipeConnector = GetPipe<T, IKeyPipeConnector<TKey>>();

            return pipeConnector.ConnectPipe(key, pipe);
        }

        protected override IOutputFilter CreateOutputPipe<T>()
        {
            var dynamicType = typeof(KeyOutputFilter<>).MakeGenericType(typeof(TInput), typeof(TKey), typeof(T));

            return (IOutputFilter)Activator.CreateInstance(dynamicType, Observers, ConverterFactory.GetConverter<T>(), _keyAccessor);
        }


        protected class KeyOutputFilter<TOutput> :
            OutputFilter<TOutput>
            where TOutput : class, TInput
        {
            public KeyOutputFilter(FilterObservable observers, IPipeContextConverter<TInput, TOutput> contextConverter, KeyAccessor<TInput, TKey> keyAccessor)
                : base(observers, contextConverter)
            {
                Filter = new OutputPipeFilter<TInput, TOutput, TKey>(ContextConverter, Observers, keyAccessor);
            }

            protected override IOutputPipeFilter<TInput, TOutput> Filter { get; }
        }
    }
}
