namespace MassTransit.Middleware
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// Handles the registration of requests and connecting them to the consume pipe
    /// </summary>
    public class RequestIdFilter<TMessage> :
        IFilter<ConsumeContext<TMessage>>,
        IKeyPipeConnector<TMessage, Guid>
        where TMessage : class
    {
        readonly ConcurrentDictionary<Guid, IPipe<ConsumeContext<TMessage>>> _pipes;

        public RequestIdFilter()
        {
            _pipes = new ConcurrentDictionary<Guid, IPipe<ConsumeContext<TMessage>>>();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("key");

            ICollection<IPipe<ConsumeContext<TMessage>>> pipes = _pipes.Values;
            scope.Add("count", pipes.Count);

            foreach (IPipe<ConsumeContext<TMessage>> pipe in pipes)
                pipe.Probe(scope);
        }

        public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Guid? key = context.RequestId;
            if (key.HasValue && _pipes.TryGetValue(key.Value, out IPipe<ConsumeContext<TMessage>> pipe))
                await pipe.Send(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public ConnectHandle ConnectPipe(Guid key, IPipe<ConsumeContext<TMessage>> pipe)
        {
            if (pipe == null)
                throw new ArgumentNullException(nameof(pipe));

            var added = _pipes.TryAdd(key, pipe);
            if (!added)
                throw new DuplicateKeyPipeConfigurationException($"A pipe with the specified key already exists: {key}");

            return new Handle(key, RemovePipe);
        }

        void RemovePipe(Guid key)
        {
            _pipes.TryRemove(key, out IPipe<ConsumeContext<TMessage>> _);
        }


        class Handle :
            ConnectHandle
        {
            readonly Guid _key;
            readonly Action<Guid> _removeKey;

            public Handle(Guid key, Action<Guid> removeKey)
            {
                _key = key;
                _removeKey = removeKey;
            }

            public void Disconnect()
            {
                _removeKey(_key);
            }

            public void Dispose()
            {
                Disconnect();
            }
        }
    }
}
