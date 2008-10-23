namespace Inventory.Messages
{
    using System;
    using MassTransit.ServiceBus;
    using MassTransit.ServiceBus.Util;

    [Serializable]
    public class QueryInventoryLevel :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;
        private readonly string _partNumber;

        public QueryInventoryLevel(string partNumber)
            : this(CombGuid.NewCombGuid(), partNumber)
        {
        }

        public QueryInventoryLevel(Guid correlationId, string partNumber)
        {
            _correlationId = correlationId;
            _partNumber = partNumber;
        }

        public string PartNumber
        {
            get { return _partNumber; }
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}