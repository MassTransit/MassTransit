namespace MassTransit.Pipeline.Sinks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Concurrency;

    /// <summary>
    /// Maintains a dictionary of valid values for the specified string expression
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to be routed</typeparam>
    /// <typeparam name="T"></typeparam>
    public class RequestMessageRouter<T, TMessage> :
        IPipelineSink<T>
        where TMessage : class
        where T : class, IMessageContext<TMessage>
    {
        readonly Func<T, string> _keyAccessor;
        readonly Atomic<Dictionary<string, MessageRouter<T>>> _output;

        public RequestMessageRouter(Func<T, string> keyAccessor)
        {
            _keyAccessor = keyAccessor;
            _output = Atomic.Create(new Dictionary<string, MessageRouter<T>>());
        }

        public IEnumerable<Action<T>> Enumerate(T context)
        {
            string key = _keyAccessor(context);

            MessageRouter<T> router;
            if (!_output.Value.TryGetValue(key, out router))
                return Enumerable.Empty<Action<T>>();

            return router.Enumerate(context);
        }

        public bool Inspect(IPipelineInspector inspector)
        {
            return inspector.Inspect(this, () => _output.Value.Values.All(x => x.Inspect(inspector)));
        }

        public int SinkCount(string key)
        {
            MessageRouter<T> router;
            if (!_output.Value.TryGetValue(key, out router))
                return 0;

            return router.SinkCount;
        }

        public UnsubscribeAction Connect(string key, IPipelineSink<T> sink)
        {
            _output.Set(sinks =>
                {
                    MessageRouter<T> router;
                    if (sinks.TryGetValue(key, out router) == false)
                    {
                        router = new MessageRouter<T>();
                        router.Connect(sink);

                        return new Dictionary<string, MessageRouter<T>>(sinks) {{key, router}};
                    }

                    var result = new Dictionary<string, MessageRouter<T>>(sinks);

                    router = new MessageRouter<T>(router.Sinks);
                    router.Connect(sink);
                    result[key] = router;

                    return result;
                });

            return () => Disconnect(key, sink);
        }

        bool Disconnect(string key, IPipelineSink<T> sink)
        {
            return _output.Set(sinks =>
                {
                    MessageRouter<T> router;
                    if (sinks.TryGetValue(key, out router) == false)
                        return sinks;

                    var result = new Dictionary<string, MessageRouter<T>>(sinks);

                    List<IPipelineSink<T>> outputSinks = router.Sinks.Where(x => x != sink).ToList();
                    if (outputSinks.Count == 0)
                        result.Remove(key);
                    else
                        result[key] = new MessageRouter<T>(outputSinks);

                    return result;
                }) != null;
        }
    }
}