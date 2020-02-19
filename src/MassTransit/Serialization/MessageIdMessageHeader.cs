namespace MassTransit.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public class MessageIdMessageHeader :
        Headers
    {
        readonly Guid _messageId;

        public MessageIdMessageHeader(Guid messageId)
        {
            _messageId = messageId;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
        {
            yield return new KeyValuePair<string, object>(nameof(MessageContext.MessageId), _messageId);
        }

        public bool TryGetHeader(string key, out object value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (key.Equals(nameof(MessageContext.MessageId)))
            {
                value = _messageId;
                return true;
            }

            value = default;
            return false;
        }

        T Headers.Get<T>(string key, T defaultValue)
        {
            if (key.Equals(nameof(MessageContext.MessageId)))
                return _messageId as T;

            return defaultValue;
        }

        public T? Get<T>(string key, T? defaultValue = null)
            where T : struct
        {
            if (key.Equals(nameof(MessageContext.MessageId)))
                return _messageId is T result
                    ? result
                    : default;

            return defaultValue;
        }

        public IEnumerator<HeaderValue> GetEnumerator()
        {
            yield return new HeaderValue(nameof(MessageContext.MessageId), _messageId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
