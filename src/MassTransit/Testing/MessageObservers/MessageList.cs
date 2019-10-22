namespace MassTransit.Testing.MessageObservers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;


    public class MessageList<TElement> :
        IEnumerable<TElement>
    {
        readonly List<Guid> _index;
        readonly Dictionary<Guid, TElement> _messages;
        readonly int _timeout;

        protected MessageList(int timeout)
        {
            _timeout = timeout;
            _messages = new Dictionary<Guid, TElement>();
            _index = new List<Guid>();
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _messages.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<TElement> Select()
        {
            return Select(x => true);
        }

        public IEnumerable<TElement> Select(Func<TElement, bool> filter)
        {
            return Select<TElement>(filter);
        }

        protected IEnumerable<T> Select<T>(Func<T, bool> filter)
        {
            lock (_messages)
            {
                int i = 0;
                for (; i < _index.Count; i++)
                {
                    if (_messages[_index[i]] is T element && filter(element))
                        yield return element;
                }

                while (Monitor.Wait(_messages, _timeout))
                {
                    for (; i < _index.Count; i++)
                    {
                        if (_messages[_index[i]] is T element && filter(element))
                            yield return element;
                    }
                }
            }
        }

        protected void Add(TElement message, Guid? messageId)
        {
            if (!messageId.HasValue)
                return;

            lock (_messages)
            {
                if (_messages.TryGetValue(messageId.Value, out _))
                {
                }
                else
                {
                    _messages.Add(messageId.Value, message);
                    _index.Add(messageId.Value);
                }

                Monitor.PulseAll(_messages);
            }
        }
    }
}
