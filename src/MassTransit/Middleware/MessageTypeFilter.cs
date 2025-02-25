namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Observables;


    /// <summary>
    /// Converts ConsumeContext to ConsumeContext&lt;T&gt; for a given message type
    /// type.
    /// </summary>
    public class ConsumeContextMessageTypeFilter :
        IConsumeContextMessageTypeFilter
    {
        readonly IPipe<ConsumeContext> _empty;
        readonly ConsumeObservable _observers;
        readonly Dictionary<Type, IOutputFilter> _outputPipes;

        IOutputFilter[] _outputPipeArray;

        public ConsumeContextMessageTypeFilter()
        {
            _outputPipes = new Dictionary<Type, IOutputFilter>();
            _outputPipeArray = [];

            _empty = Pipe.Empty<ConsumeContext>();

            _observers = new ConsumeObservable();
        }

        public void Probe(ProbeContext context)
        {
            foreach (var pipe in _outputPipes.Values)
                pipe.Probe(context);
        }

        public ConnectHandle ConnectMessagePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return GetMessagePipe<T>().Filter.ConnectPipe(pipe);
        }

        public ConnectHandle ConnectMessagePipe<T>(Guid key, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            return GetMessagePipe<T>().Filter.ConnectPipe(key, pipe);
        }

        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
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

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return GetMessagePipe<T>().Filter.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _observers.Connect(observer);
        }

        OutputFilter<T> GetMessagePipe<T>()
            where T : class
        {
            lock (_outputPipes)
            {
                if (_outputPipes.TryGetValue(typeof(T), out var outputPipe))
                    return (OutputFilter<T>)outputPipe;

                OutputFilter<T> newOutputPipe = CreateOutputPipe<T>();

                _outputPipes.Add(typeof(T), newOutputPipe);

                _outputPipeArray = _outputPipes.Values.ToArray();

                return newOutputPipe;
            }
        }

        OutputFilter<T> CreateOutputPipe<T>()
            where T : class
        {
            return new OutputFilter<T>(_observers);
        }


        interface IOutputFilter :
            IFilter<ConsumeContext>
        {
        }


        class OutputFilter<TMessage> :
            IOutputFilter
            where TMessage : class
        {
            public OutputFilter(ConsumeObservable observers)
            {
                Filter = new ConsumeContextOutputMessageTypeFilter<TMessage>(observers, new RequestIdTeeFilter<TMessage>());
            }

            public virtual ConsumeContextOutputMessageTypeFilter<TMessage> Filter { get; }

            public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
            {
                return Filter.Send(context, next);
            }

            public void Probe(ProbeContext context)
            {
                Filter.Probe(context);
            }
        }
    }
}
