namespace MassTransit.ServiceBus.Services.MessageDeferral.Messages
{
    using System;

    [Serializable]
    public class DeferedMessageRepublished
    {
        private readonly Guid _deferredMessageId;

        public DeferedMessageRepublished(Guid deferredMessageId)
        {
            _deferredMessageId = deferredMessageId;
        }

        public Guid DeferredMessageId
        {
            get { return _deferredMessageId; }
        }
    }
}