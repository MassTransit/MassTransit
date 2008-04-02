using MassTransit.Patterns.Batching;
using MassTransit.ServiceBus;

namespace MassTransit.Patterns.Tests
{
    using System;

    [Serializable]
    public class MessageToBatch : IMessage
    {
        public string Name;
    }
}