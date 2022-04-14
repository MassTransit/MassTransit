namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Collections.Generic;


    public readonly struct InMemoryInboxMessageKey
    {
        public readonly Guid MessageId;
        public readonly Guid ConsumerId;

        public InMemoryInboxMessageKey(Guid messageId, Guid consumerId)
        {
            MessageId = messageId;
            ConsumerId = consumerId;
        }


        sealed class EqualityComparer :
            IEqualityComparer<InMemoryInboxMessageKey>
        {
            public bool Equals(InMemoryInboxMessageKey x, InMemoryInboxMessageKey y)
            {
                return x.MessageId.Equals(y.MessageId) && x.ConsumerId.Equals(y.ConsumerId);
            }

            public int GetHashCode(InMemoryInboxMessageKey obj)
            {
                unchecked
                {
                    return (obj.MessageId.GetHashCode() * 397) ^ obj.ConsumerId.GetHashCode();
                }
            }
        }


        public static IEqualityComparer<InMemoryInboxMessageKey> Comparer { get; } = new EqualityComparer();
    }
}
