namespace MassTransit.Transports.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Util;


    public class TopicNode<T> :
        IProbeSite
        where T : class
    {
        readonly ConcurrentDictionary<string, TopicNode<T>> _children;
        readonly StringComparer _comparer;
        readonly Connectable<IMessageSink<T>> _sinks;

        public TopicNode(StringComparer comparer)
        {
            _comparer = comparer;

            _sinks = new Connectable<IMessageSink<T>>();
            _children = new ConcurrentDictionary<string, TopicNode<T>>(comparer);
        }

        public IEnumerable<IMessageSink<T>> Sinks
        {
            get
            {
                var sinks = new List<IMessageSink<T>>();

                _sinks.ForEach(s => sinks.Add(s));

                foreach (TopicNode<T> node in _children.Values)
                    sinks.AddRange(node.Sinks);

                return sinks.Distinct();
            }
        }

        public void Probe(ProbeContext context)
        {
            _sinks.ForEach(s => s.Probe(context));

            foreach (KeyValuePair<string, TopicNode<T>> node in _children)
            {
                var nodeScope = context.CreateScope(node.Key);

                node.Value.Probe(nodeScope);
            }
        }

        public ConnectHandle Add(IMessageSink<T> sink, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return _sinks.Connect(sink);

            var separator = pattern.IndexOf('.');
            if (separator < 0)
                return GetChild(pattern).Bind(sink);

            var word = pattern.Substring(0, separator);

            return GetChild(word).Add(sink, pattern, separator + 1);
        }

        public async Task Deliver(DeliveryContext<T> context, string routingKey)
        {
            if (_children.TryGetValue("#", out TopicNode<T> hashNode))
                await hashNode.Deliver(context, default).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(routingKey))
            {
                await _sinks.ForEachAsync(async sink =>
                {
                    if (context.WasAlreadyDelivered(sink))
                        return;

                    await sink.Deliver(context).ConfigureAwait(false);

                    context.Delivered(sink);
                }).ConfigureAwait(false);
            }
            else
            {
                var separator = routingKey.IndexOf('.');
                if (separator < 0)
                {
                    if (_children.TryGetValue("*", out TopicNode<T> starNode))
                        await starNode.Deliver(context, default).ConfigureAwait(false);

                    if (_children.TryGetValue(routingKey, out TopicNode<T> childNode))
                        await childNode.Deliver(context, default).ConfigureAwait(false);
                }
                else
                {
                    var word = routingKey.Substring(0, separator);

                    var remaining = routingKey.Substring(separator + 1);

                    if (_children.TryGetValue("*", out TopicNode<T> starNode))
                        await starNode.Deliver(context, remaining).ConfigureAwait(false);

                    if (_children.TryGetValue(word, out TopicNode<T> childNode))
                        await childNode.Deliver(context, remaining).ConfigureAwait(false);
                }
            }
        }

        ConnectHandle Add(IMessageSink<T> sink, string pattern, int offset)
        {
            var separator = pattern.IndexOf('.', offset);
            if (separator < 0)
                return GetChild(pattern.Substring(offset)).Bind(sink);

            var word = pattern.Substring(offset, separator - offset);

            return GetChild(word).Add(sink, pattern, separator + 1);
        }

        TopicNode<T> GetChild(string word)
        {
            return _children.GetOrAdd(word, _ => new TopicNode<T>(_comparer));
        }

        ConnectHandle Bind(IMessageSink<T> sink)
        {
            return _sinks.Connect(sink);
        }
    }
}
