namespace CodeCamp.Messages
{
    using System;
    using MassTransit.ServiceBus;

    [Serializable]
    public class UserVerificationEmailSent :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;

        public UserVerificationEmailSent(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}