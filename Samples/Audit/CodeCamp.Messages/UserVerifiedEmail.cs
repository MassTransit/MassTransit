namespace CodeCamp.Messages
{
    using System;
    using MassTransit;

    [Serializable]
    public class UserVerifiedEmail :
        CorrelatedBy<Guid>
    {
        private readonly Guid _correlationId;

        public UserVerifiedEmail(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }
    }
}