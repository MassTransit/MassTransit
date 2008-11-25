namespace HeavyLoad.Correlated
{
    using System;
    using MassTransit;

    [Serializable]
    internal class SimpleResponseMessage : 
        CorrelatedBy<Guid>
    {
        private Guid _id;

        public SimpleResponseMessage()
        {
        }

        public SimpleResponseMessage(Guid id)
        {
            _id = id;
        }

        public Guid CorrelationId
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}