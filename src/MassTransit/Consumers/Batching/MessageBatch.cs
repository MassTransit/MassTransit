namespace MassTransit.Batching
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    [Serializable]
    public class MessageBatch<TMessage> :
        Batch<TMessage>
        where TMessage : class
    {
        readonly IReadOnlyList<ConsumeContext<TMessage>> _messages;

        public MessageBatch(DateTime firstMessageReceived, DateTime lastMessageReceived, BatchCompletionMode mode,
            IReadOnlyList<ConsumeContext<TMessage>> messages)
        {
            FirstMessageReceived = firstMessageReceived;
            LastMessageReceived = lastMessageReceived;
            Mode = mode;
            _messages = messages;
        }

        public BatchCompletionMode Mode { get; set; }
        public DateTime FirstMessageReceived { get; set; }
        public DateTime LastMessageReceived { get; set; }

        public ConsumeContext<TMessage> this[int index] => _messages[index];

        public int Length => _messages.Count;

        public IEnumerator<ConsumeContext<TMessage>> GetEnumerator()
        {
            return _messages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
