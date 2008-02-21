namespace MassTransit.Patterns.Tests
{
    using System;
    using Batching;

    [Serializable]
    public class MessageToBatch :
        BatchMessage<Guid>
    {
        public MessageToBatch(Guid batchId, int batchLength)
            : base(batchId, batchLength)
        {
        }
    }
}