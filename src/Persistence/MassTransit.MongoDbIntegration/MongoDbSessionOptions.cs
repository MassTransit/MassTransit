#nullable enable
namespace MassTransit
{
    using System;


    public class MongoDbSessionOptions
    {
        public TimeSpan LockRetryDelay { get; set; } = TimeSpan.FromMilliseconds(20);
    }
}
