#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, string name, TimeSpan? autoDeleteOnIdle, int? maxDeliveryCount)
        {
            Id = id;
            QueueName = name;
            AutoDeleteOnIdle = autoDeleteOnIdle;
            MaxDeliveryCount = maxDeliveryCount;
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<QueueEntity> QueueComparer { get; } = new QueueEntityEqualityComparer();

        public string QueueName { get; }
        public TimeSpan? AutoDeleteOnIdle { get; }
        public int? MaxDeliveryCount { get;}
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[] { $"name: {QueueName}", AutoDeleteOnIdle.HasValue ? $"auto-delete after {AutoDeleteOnIdle}" : "", }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueEntityEqualityComparer : IEqualityComparer<QueueEntity>
        {
            public bool Equals(QueueEntity? x, QueueEntity? y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.QueueName, y.QueueName) && x.AutoDeleteOnIdle == y.AutoDeleteOnIdle;
            }

            public int GetHashCode(QueueEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.QueueName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.AutoDeleteOnIdle.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer : IEqualityComparer<QueueEntity>
        {
            public bool Equals(QueueEntity? x, QueueEntity? y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.QueueName, y.QueueName);
            }

            public int GetHashCode(QueueEntity obj)
            {
                return obj.QueueName.GetHashCode();
            }
        }
    }
}
