namespace MassTransit.GrpcTransport.Fabric
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;


    public class TopicNode :
        IProbeSite
    {
        readonly ConcurrentDictionary<string, TopicNode> _children;
        readonly StringComparer _comparer;
        readonly Connectable<IMessageSink<GrpcTransportMessage>> _sinks;

        public TopicNode(StringComparer comparer)
        {
            _comparer = comparer;

            _sinks = new Connectable<IMessageSink<GrpcTransportMessage>>();
            _children = new ConcurrentDictionary<string, TopicNode>(comparer);
        }

        public void Probe(ProbeContext context)
        {
            _sinks.All(s =>
            {
                s.Probe(context);

                return true;
            });

            foreach (KeyValuePair<string, TopicNode> node in _children)
            {
                var nodeScope = context.CreateScope(node.Key);

                node.Value.Probe(nodeScope);
            }
        }

        public ConnectHandle Add(IMessageSink<GrpcTransportMessage> sink, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return _sinks.Connect(sink);

            var separator = pattern.IndexOf('.');
            if (separator < 0)
                return GetChild(pattern).Bind(sink);

            var word = pattern.Substring(0, separator);

            return GetChild(word).Add(sink, pattern, separator + 1);
        }

        public async Task Deliver(DeliveryContext<GrpcTransportMessage> context, string routingKey)
        {
            if (_children.TryGetValue("#", out var hashNode))
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
                    if (_children.TryGetValue("*", out var starNode))
                        await starNode.Deliver(context, default).ConfigureAwait(false);

                    if (_children.TryGetValue(routingKey, out var childNode))
                        await childNode.Deliver(context, default).ConfigureAwait(false);
                }
                else
                {
                    var word = routingKey.Substring(0, separator);

                    var remaining = routingKey.Substring(separator + 1);

                    if (_children.TryGetValue("*", out var starNode))
                        await starNode.Deliver(context, remaining).ConfigureAwait(false);

                    if (_children.TryGetValue(word, out var childNode))
                        await childNode.Deliver(context, remaining).ConfigureAwait(false);
                }
            }
        }

        public IEnumerable<IMessageSink<GrpcTransportMessage>> Sinks
        {
            get
            {
                var sinks = new List<IMessageSink<GrpcTransportMessage>>();

                _sinks.All(s =>
                {
                    sinks.Add(s);

                    return true;
                });

                foreach (var node in _children.Values)
                {
                    sinks.AddRange(node.Sinks);
                }

                return sinks.Distinct();
            }
        }

        ConnectHandle Add(IMessageSink<GrpcTransportMessage> sink, string pattern, int offset)
        {
            var separator = pattern.IndexOf('.', offset);
            if (separator < 0)
                return GetChild(pattern.Substring(offset)).Bind(sink);

            var word = pattern.Substring(offset, separator - offset);

            return GetChild(word).Add(sink, pattern, separator + 1);
        }

        TopicNode GetChild(string word)
        {
            return _children.GetOrAdd(word, _ => new TopicNode(_comparer));
        }

        ConnectHandle Bind(IMessageSink<GrpcTransportMessage> sink)
        {
            return _sinks.Connect(sink);
        }
    }
}
