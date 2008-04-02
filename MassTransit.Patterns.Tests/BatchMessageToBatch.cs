using System;
using MassTransit.Patterns.Batching;

namespace MassTransit.Patterns.Tests
{
    [Serializable]
    public class BatchMessageToBatch : BatchMessage<MessageToBatch, Guid>
    {
        public BatchMessageToBatch(Guid batchId, int batchLength, MessageToBatch body) : base(batchId, batchLength, body)
        {
        }

        public BatchMessageToBatch()
        {
        }
    }
}