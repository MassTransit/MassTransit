namespace MassTransit.Middleware
{
    using System;
    using System.Threading.Tasks;
    using Observables;


    /// <summary>
    /// Converts an inbound context type to a pipe context type post-dispatch
    /// </summary>
    /// <typeparam name="TMessage">The subsequent pipe context type</typeparam>
    public class ConsumeContextOutputMessageTypeFilter<TMessage> :
        IConsumeContextOutputMessageTypeFilter<TMessage>
        where TMessage : class
    {
        readonly ConsumeObservable _consumeObservers;
        readonly ConsumeMessageObservable<TMessage> _observers;
        readonly IRequestIdTeeFilter<TMessage> _output;

        public ConsumeContextOutputMessageTypeFilter(ConsumeObservable observers, IRequestIdTeeFilter<TMessage> output)
        {
            _output = output;

            _consumeObservers = observers;
            _observers = new ConsumeMessageObservable<TMessage>();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("dispatchPipe");
            scope.Add("outputType", TypeCache<ConsumeContext<TMessage>>.ShortName);

            _output.Probe(scope);
        }

        public Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
        {
            return context.TryGetMessage(out ConsumeContext<TMessage> pipeContext)
                ? SendToOutput(next, pipeContext)
                : next.Send(context);
        }

        public ConnectHandle ConnectConsumeMessageObserver(IConsumeMessageObserver<TMessage> observer)
        {
            return _observers.Connect(observer);
        }

        public ConnectHandle ConnectPipe(IPipe<ConsumeContext<TMessage>> pipe)
        {
            return _output.ConnectPipe(pipe);
        }

        public ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
        {
            return _output.ConnectPipe(key, pipe);
        }

        async Task SendToOutput(IPipe<ConsumeContext> next, ConsumeContext<TMessage> pipeContext)
        {
            if (_observers.Count > 0)
            {
                var preConsumeTask = _observers.PreConsume(pipeContext);
                if (preConsumeTask.Status != TaskStatus.RanToCompletion)
                    await preConsumeTask.ConfigureAwait(false);
            }

            if (_consumeObservers.Count > 0)
            {
                var preConsumeTask = _consumeObservers.PreConsume(pipeContext);
                if (preConsumeTask.Status != TaskStatus.RanToCompletion)
                    await preConsumeTask.ConfigureAwait(false);
            }

            try
            {
                await _output.Send(pipeContext, next).ConfigureAwait(false);

                if (_observers.Count > 0)
                {
                    var postConsumeTask = _observers.PostConsume(pipeContext);
                    if (postConsumeTask.Status != TaskStatus.RanToCompletion)
                        await postConsumeTask.ConfigureAwait(false);
                }

                if (_consumeObservers.Count > 0)
                {
                    var postConsumeTask = _consumeObservers.PostConsume(pipeContext);
                    if (postConsumeTask.Status != TaskStatus.RanToCompletion)
                        await postConsumeTask.ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (_observers.Count > 0)
                {
                    var consumeFaultTask = _observers.ConsumeFault(pipeContext, ex);
                    if (consumeFaultTask.Status != TaskStatus.RanToCompletion)
                        await consumeFaultTask.ConfigureAwait(false);
                }

                if (_consumeObservers.Count > 0)
                {
                    var consumeFaultTask = _consumeObservers.ConsumeFault(pipeContext, ex);
                    if (consumeFaultTask.Status != TaskStatus.RanToCompletion)
                        await consumeFaultTask.ConfigureAwait(false);
                }

                throw;
            }
        }
    }
}
