namespace Inventory.Messages
{
    using System;
    using MassTransit;

    [Serializable]
    public class PartInventoryLevelStatus :
        CorrelatedBy<string>
    {
        private readonly int _onHand;
        private readonly int _onOrder;
        private readonly string _partNumber;

        public PartInventoryLevelStatus(string partNumber, int onHand, int onOrder)
        {
            _partNumber = partNumber;
            _onHand = onHand;
            _onOrder = onOrder;
        }

        public int OnOrder
        {
            get { return _onOrder; }
        }

        public int OnHand
        {
            get { return _onHand; }
        }

        public string PartNumber
        {
            get { return _partNumber; }
        }

        public string CorrelationId
        {
            get { return _partNumber; }
        }
    }
}